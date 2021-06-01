using Microsoft.EntityFrameworkCore;
using Mifs;
using Mifs.Extensions;
using Mifs.MEX;
using Mifs.MEX.Constants;
using Mifs.MEX.Services;
using Mifs.Scheduling;
using Mifs.Xero.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using MEXContact = Mifs.MEX.Domain.Contact;
using XeroAddressTypeEnum = Xero.NetStandard.OAuth2.Model.Accounting.Address.AddressTypeEnum;
using XeroContact = Xero.NetStandard.OAuth2.Model.Accounting.Contact;
using XeroPhoneTypeEnum = Xero.NetStandard.OAuth2.Model.Accounting.Phone.PhoneTypeEnum;

namespace XeroIntegration.Import
{
    public class XeroSupplierImport : IJobInitializer, IScheduledDataProvider<XeroContact>, IProcessEntity<XeroContact>
    {
        public XeroSupplierImport(IAccountingApiService accountingApi,
                                IMEXDbContext mexDbContext,
                                IAccountCodeService accountCodeService)
        {
            this.AccountingApi = accountingApi;
            this.MexDbContext = mexDbContext;
            this.AccountCodeService = accountCodeService;
        }

        private IAccountingApiService AccountingApi { get; }
        private IMEXDbContext MexDbContext { get; }
        private IAccountCodeService AccountCodeService { get; }

        private IEnumerable<MEXContact> MEXSuppliers { get; set; } = new List<MEXContact>();

        public async Task Initialize(CancellationToken cancellationToken)
        {
            this.MEXSuppliers = await this.MexDbContext.Contacts.Where(c => c.ContactTypeName == "Supplier")
                                                                .Where(c => c.ParentContactID == 0)
                                                                .ToListAsync(cancellationToken);
        }

        public async IAsyncEnumerable<XeroContact> GetData([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var allXeroContacts = await this.AccountingApi.GetContacts();
            allXeroContacts = allXeroContacts.Where(x => x.IsSupplier ?? false)
                                             .Where(x => !x.Name.IsNullOrWhiteSpace());

            foreach (var xeroContact in allXeroContacts)
            {
                yield return xeroContact;
            }
        }

        public Task<bool> HandleError(XeroContact entity, Exception exception, CancellationToken cancellationToken)
        {
            return Task.FromResult(false);
        }

        public async Task Process(XeroContact xeroSupplier, CancellationToken cancellationToken)
        {
            var mexSupplier = this.MEXSuppliers.FirstOrDefault(c => c.FirstName == xeroSupplier.Name);
            if (mexSupplier is null)
            {
                mexSupplier = new MEXContact()
                {
                    FirstName = xeroSupplier.Name,
                    ContactTypeName = ContactType.Supplier,
                    IsActive = true
                };

                this.MexDbContext.Contacts.Add(mexSupplier);
            }

            mexSupplier.Email = xeroSupplier.EmailAddress;
            mexSupplier.WebAddress = xeroSupplier.Website;
            mexSupplier.ABN = xeroSupplier.TaxNumber;

            var xeroAddress = xeroSupplier.Addresses.FirstOrDefault(x => x.AddressType == XeroAddressTypeEnum.POBOX);
            if (xeroAddress is not null)
            {
                mexSupplier.Address1 = xeroAddress.AddressLine1;
                mexSupplier.Address2 = xeroAddress.AddressLine2;
                mexSupplier.City = xeroAddress.City;
                mexSupplier.Country = xeroAddress.Country;
                mexSupplier.State = xeroAddress.Region;
                mexSupplier.City = xeroAddress.City;
                mexSupplier.PostCode = xeroAddress.PostalCode;
            }

            if (!xeroSupplier.PurchasesDefaultAccountCode.IsNullOrWhiteSpace())
            {
                mexSupplier.AccountCode = await this.AccountCodeService.GetOrCreateAccountCode(xeroSupplier.PurchasesDefaultAccountCode,
                                                                                               xeroSupplier.PurchasesDefaultAccountCode,
                                                                                               this.MexDbContext,
                                                                                               cancellationToken);
            }

            foreach (var contactPhone in xeroSupplier.Phones)
            {
                var mexPhoneNumer = $"{contactPhone.PhoneCountryCode} {contactPhone.PhoneAreaCode} {contactPhone.PhoneNumber}";

                switch (contactPhone.PhoneType)
                {
                    case XeroPhoneTypeEnum.DEFAULT:
                        mexSupplier.HomePhone = mexPhoneNumer;
                        break;
                    case XeroPhoneTypeEnum.DDI:
                        mexSupplier.WorkPhone = mexPhoneNumer;
                        break;
                    case XeroPhoneTypeEnum.FAX:
                        mexSupplier.Fax = mexPhoneNumer;
                        break;
                    case XeroPhoneTypeEnum.MOBILE:
                        mexSupplier.MobilePhone = mexPhoneNumer;
                        break;
                }
            }

            await this.MexDbContext.SaveChangesAsync(cancellationToken);
        }

    }
}

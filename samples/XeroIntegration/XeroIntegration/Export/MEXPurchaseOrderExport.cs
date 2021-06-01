using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Mifs;
using Mifs.Extensions;
using Mifs.MEX;
using Mifs.MEX.Constants;
using Mifs.MEX.Domain;
using Mifs.MEX.Logging;
using Mifs.Scheduling;
using Mifs.Xero.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using XeroLineItem = Xero.NetStandard.OAuth2.Model.Accounting.LineItem;
using XeroPurchaseOrder = Xero.NetStandard.OAuth2.Model.Accounting.PurchaseOrder;

namespace XeroIntegration.Export
{
    public class MEXPurchaseOrderExport : IScheduledDataProvider<PurchaseOrder>, IProcessEntity<PurchaseOrder>
    {
        private IFactory<IMEXDbContext> MexDbContextFactory { get; }
        private IMEXDbContext QueryDbContext { get; }

        private IAccountingApiService AccountingApi { get; }
        private IMapper Mapper { get; }
        private IProcessedEntityLogger<PurchaseOrder> ProcessedEntityLogger { get; }

        public MEXPurchaseOrderExport(
            IFactory<IMEXDbContext> mexDbContextFactory,
            IMEXDbContext queryDbContext,
            IAccountingApiService accountingApi,
            IMapper mapper,
            IProcessedEntityLogger<PurchaseOrder> processedEntityLogger)
        {
            this.MexDbContextFactory = mexDbContextFactory;
            this.QueryDbContext = queryDbContext;
            this.AccountingApi = accountingApi;
            this.Mapper = mapper;
            this.ProcessedEntityLogger = processedEntityLogger;
        }

        public IAsyncEnumerable<PurchaseOrder> GetData(CancellationToken cancellationToken)
            => this.QueryDbContext.PurchaseOrders
                                  .AsNoTracking()
                                  .Include(po => po.SupplierContact)
                                  .Include(po => po.Extension_PurchaseOrder)
                                  .Include(po => po.CurrencyType)
                                  .Where(po => !po.IsCancelled)
                                  .FilterApprovalStatus(this.QueryDbContext, ApprovalStatus.Approved)
                                  .FilterProcessed("Demo Integration", po => po.Extension_PurchaseOrder.LastApprovalStatusSetDateTime, this.QueryDbContext)
                                  .AsAsyncEnumerable();


        public async Task<bool> HandleError(PurchaseOrder po, Exception exception, CancellationToken cancellationToken)
        {
            await this.ProcessedEntityLogger.LogFailure(po.PurchaseOrderID, $"{exception.Message}", cancellationToken);
            return true;
        }


        public async Task Process(PurchaseOrder mexPo, CancellationToken cancellationToken)
        {
            var xeroPo = await this.GenerateXeroPurchaseOrder(mexPo, cancellationToken);

            await this.AccountingApi.CreatePurchaseOrderWithHistory(xeroPo);
            await this.ProcessedEntityLogger.LogSuccess(mexPo.Id, cancellationToken);
        }

        private async Task<XeroPurchaseOrder?> GenerateXeroPurchaseOrder(PurchaseOrder mexPo, CancellationToken cancellationToken)
        {
            var mexSupplier = mexPo.SupplierContact;
            var xeroContact = await this.AccountingApi.GetContacts(where: $"Name==\"{mexSupplier?.FirstName}\"")
                                                      .FirstOrDefault();

            if (xeroContact is null)
            {
                throw new InvalidOperationException($"Unable to find Xero Supplier with name {mexSupplier?.FirstName} for Purchase Order {mexPo.PurchaseOrderNumber}.");
            }

            var xeroPo = this.Mapper.Map<PurchaseOrder, XeroPurchaseOrder>(mexPo);

            var xeroAccounts = await this.AccountingApi.GetAccounts();
            var xeroLines = new List<XeroLineItem>();

            using var mexDbContext = this.MexDbContextFactory.Create();
            if (mexDbContext is null)
            {
                return null;
            }

            var poLines = this.GetPurchaseOrderLinesForPo(mexPo, mexDbContext);

            await foreach (var poLine in poLines.WithCancellation(cancellationToken))
            {
                var accountCodeName = poLine.AccountCode?.AccountCodeName;
                var xeroLine = this.Mapper.Map<PurchaseOrderLine, XeroLineItem>(poLine);

                if (!poLine.Extension_PurchaseOrderLine.WorkOrderNumber.IsNullOrWhiteSpace())
                {
                    xeroLine.Description = xeroLine.Description.Join(" - ", $"WO#: {poLine.Extension_PurchaseOrderLine.WorkOrderNumber}");
                }

                if (poLine.TaxPercentage != 0 && !accountCodeName.IsNullOrWhiteSpace())
                {
                    xeroLine.TaxType = xeroAccounts.FirstOrDefault(acc => acc.Code == accountCodeName)?.TaxType;
                }

                xeroLines.Add(xeroLine);
            }

            if (mexPo.FreightAmount > 0)
            {
                var freightAccount = xeroAccounts.FirstOrDefault(acc => acc.Name?.Contains("Freight") == true);
                var xeroLine = new XeroLineItem()
                {
                    Quantity = 1,
                    UnitAmount = mexPo.FreightAmount,
                    Description = "Freight Line",
                    AccountCode = freightAccount?.Code,
                    TaxType = freightAccount?.TaxType,
                    TaxAmount = mexPo.FreightAmount * mexPo.FreightTaxPercentage
                };

                xeroLines.Add(xeroLine);
            }

            xeroPo.LineItems = xeroLines;
            return xeroPo;
        }

        private IAsyncEnumerable<PurchaseOrderLine> GetPurchaseOrderLinesForPo(PurchaseOrder purchaseOrder, IMEXDbContext dbContext)
            => dbContext.PurchaseOrderLines.Include(x => x.AccountCode)
                                           .Include(x => x.Extension_PurchaseOrderLine)
                                           .Where(poLine => poLine.PurchaseOrderID == purchaseOrder.PurchaseOrderID)
                                           .AsAsyncEnumerable();
    }
}
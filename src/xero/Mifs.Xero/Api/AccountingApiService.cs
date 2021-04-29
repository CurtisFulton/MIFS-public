using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xero.NetStandard.OAuth2.Model.Accounting;

namespace Mifs.Xero.Api
{
    public class AccountingApiService : IAccountingApiService
    {
        public Task<IEnumerable<Account>> GetAccounts(string where = null)
        {
            return Task.FromResult<IEnumerable<Account>>(new List<Account> { new Account() });
        }

        public Task<IEnumerable<Contact>> GetContacts(string where = null)
        {
            return Task.FromResult<IEnumerable<Contact>>(new List<Contact> { new Contact() });
        }

        public Task<IEnumerable<PurchaseOrder>> CreatePurchaseOrders(IEnumerable<PurchaseOrder> purchaseOrders)
        {
            return Task.FromResult(purchaseOrders);
        }

        public Task<IEnumerable<HistoryRecord>> CreatePurchaseOrderHistory(Guid purchaseOrderId, IEnumerable<HistoryRecord> historyRecords)
        {
            return Task.FromResult(historyRecords);
        }
    }
}

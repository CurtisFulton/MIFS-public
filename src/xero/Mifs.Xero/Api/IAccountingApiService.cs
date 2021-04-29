using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xero.NetStandard.OAuth2.Model.Accounting;

namespace Mifs.Xero.Api
{
    public interface IAccountingApiService
    {
        Task<IEnumerable<Account>> GetAccounts(string where = null);
        Task<IEnumerable<Contact>> GetContacts(string where = null);

        Task<IEnumerable<PurchaseOrder>> CreatePurchaseOrders(IEnumerable<PurchaseOrder> purchaseOrders);
        Task<IEnumerable<HistoryRecord>> CreatePurchaseOrderHistory(Guid purchaseOrderId, IEnumerable<HistoryRecord> historyRecords);
    }
}

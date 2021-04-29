using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xero.NetStandard.OAuth2.Model.Accounting;

namespace Mifs.Xero.Api
{
    public static class AccountingApiService_Extensions
    {
        public static async Task<PurchaseOrder> CreatePurchaseOrderWithHistory(this IAccountingApiService accountingApiService, PurchaseOrder purchaseOrder)
        {
            var response = await accountingApiService.CreatePurchaseOrders(new List<PurchaseOrder>() { purchaseOrder });

            var createdPurchaseOrder = response.FirstOrDefault();
            if (createdPurchaseOrder?.PurchaseOrderID != null)
            {
                var historyRecord = new HistoryRecord()
                {
                    User = "MEX",
                    Details = "PO Created from MEX",
                    Changes = "PO Created from MEX"
                };

                await accountingApiService.CreatePurchaseOrderHistory(createdPurchaseOrder.PurchaseOrderID.Value, historyRecord);
            }

            return createdPurchaseOrder;
        }

        public static async Task<PurchaseOrder> CreatePurchaseOrder(this IAccountingApiService accountingApiService, PurchaseOrder purchaseOrder)
        {
            var response = await accountingApiService.CreatePurchaseOrders(new List<PurchaseOrder>() { purchaseOrder });
            return response.FirstOrDefault();
        }

        public static async Task<HistoryRecord> CreatePurchaseOrderHistory(this IAccountingApiService accountingApiService, Guid purchaseOrderId, HistoryRecord historyRecord)
        {
            var response = await accountingApiService.CreatePurchaseOrderHistory(purchaseOrderId, new List<HistoryRecord>() { historyRecord });
            return response.FirstOrDefault();
        }
    }
}

using Mifs.MEX.Api;
using Mifs.MEX.Domain;
using System.Threading.Tasks;

namespace Mifs.MEX.Services
{
    public interface IPurchaseOrderApprovalService
    {
        Task<ODataFunctionImportQueryableData> ApprovePurchaseOrder(PurchaseOrder purchaseOrder);
        Task<ODataFunctionImportQueryableData> DeclinePurchaseOrder(PurchaseOrder purchaseOrder);

        Task SetPurchaseOrderApprovalStatus(PurchaseOrder purchaseOrder, string approvalStatus);
    }

    public class PurchaseOrderApprovalService : IPurchaseOrderApprovalService
    {
        public Task<ODataFunctionImportQueryableData> ApprovePurchaseOrder(PurchaseOrder purchaseOrder)
        {
            throw new System.NotImplementedException();
        }

        public Task<ODataFunctionImportQueryableData> DeclinePurchaseOrder(PurchaseOrder purchaseOrder)
        {
            throw new System.NotImplementedException();
        }

        public Task SetPurchaseOrderApprovalStatus(PurchaseOrder purchaseOrder, string approvalStatus)
        {
            return Task.CompletedTask;
        }
    }
}

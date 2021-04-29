using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mifs.MEX.Api
{
    public class PurchaseOrderApi
    {
        public PurchaseOrderApi(IServiceOpService serviceOpService)
        {
            this.ServiceOpService = serviceOpService;
        }

        private IServiceOpService ServiceOpService { get; }

        public async Task<ODataFunctionImportQueryableData> ApprovePurchaseOrder(int purchaseOrderNumber, bool skipApprovalMessage, CancellationToken cancellationToken)
        {
            var skipApprovalMessageInteger = Convert.ToInt32(skipApprovalMessage);
            return await this.ServiceOpService.Execute<ODataFunctionImportQueryableData>("ApprovePurchaseOrder", cancellationToken: cancellationToken, purchaseOrderNumber, skipApprovalMessageInteger);
        }
    }
}

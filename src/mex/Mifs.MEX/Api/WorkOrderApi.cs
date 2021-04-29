using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Mifs.MEX.Api
{
    public class WorkOrderApi
    {
        public WorkOrderApi(IServiceOpService serviceOpService)
        {
            this.ServiceOpService = serviceOpService;
        }

        private IServiceOpService ServiceOpService { get; }

        public Task<ODataFunctionImportQueryableData> CloseWorkOrder(int workOrderId, bool completeRequests, CancellationToken cancellationToken)
            => this.CloseWorkOrders(new List<int> { workOrderId }, completeRequests, cancellationToken);

        public async Task<ODataFunctionImportQueryableData> CloseWorkOrders(IEnumerable<int> workOrderIds, bool completeRequests, CancellationToken cancellationToken)
        {
            var workOrderIdStringList = workOrderIds.AsMEXServiceOpParameterList();
            var completeRequestsString = completeRequests.ToString();

            return await this.ServiceOpService.Execute<ODataFunctionImportQueryableData>("CloseWorkOrdersToHistory", cancellationToken, workOrderIdStringList, completeRequestsString);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Mifs.MEX.Api
{
    public class AssetReadingApi
    {
        public AssetReadingApi(IServiceOpService serviceOpService)
        {
            this.ServiceOpService = serviceOpService;
        }

        private IServiceOpService ServiceOpService { get; }

        public async Task ProcessAssetReadingRecordings(IEnumerable<int> assetReadingIds, 
                                                        IEnumerable<decimal> newReadings, 
                                                        IEnumerable<DateTime> newDateTimes, 
                                                        IEnumerable<decimal> diffBeforeDelete, 
                                                        bool isUpdateComponents, 
                                                        CancellationToken cancellationToken)
        {
            var assetReadingIdsString = assetReadingIds.AsMEXServiceOpParameterList();
            var newReadingsString = newReadings.AsMEXServiceOpParameterList();
            var newDateTimesString = newDateTimes.AsMEXServiceOpParameterList();
            var diffBeforeDeleteString = diffBeforeDelete.AsMEXServiceOpParameterList();
            var isUpdateComponentsString = isUpdateComponents.ToString();

            await this.ServiceOpService.Execute("CloseWorkOrdersToHistory",
                                                cancellationToken,
                                                assetReadingIdsString,
                                                newReadingsString,
                                                newDateTimesString,
                                                diffBeforeDeleteString,
                                                isUpdateComponentsString);
        }
    }
}

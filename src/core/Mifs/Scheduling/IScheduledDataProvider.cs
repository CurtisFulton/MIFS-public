using System;
using System.Collections.Generic;
using System.Threading;

namespace Mifs.Scheduling
{
    public interface IScheduledDataProvider<TData>
    {
        IAsyncEnumerable<TData> GetData(CancellationToken cancellationToken);
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mifs.Scheduling
{
    /// <summary>
    /// Provides an implementation for processing a single entity.
    /// A separate 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IProcessEntity<TEntity>
    {
        Task Process(TEntity entity, CancellationToken cancellationToken);
        Task<bool> HandleError(TEntity entity, Exception exception, CancellationToken cancellationToken);
    }
}

using System.Threading;
using System.Threading.Tasks;

namespace Mifs.MEX.Logging
{
    public interface IProcessedEntityLogger<TEntity>
    {
        Task<int> LogSuccess(int entityId, CancellationToken cancellationToken = default);
        Task<int> LogFailure(int entityId, string failureMessage, CancellationToken cancellationToken = default);
    }
}

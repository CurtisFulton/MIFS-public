using System.Threading;
using System.Threading.Tasks;

namespace Mifs
{
    /// <summary>
    /// Interface used to run initialization logic for a job.
    /// </summary>
    /// Kept this specifically simple so it can be used for multiple things
    public interface IJobInitializer 
    {
        Task Initialize(CancellationToken cancellationToken);
    }
}

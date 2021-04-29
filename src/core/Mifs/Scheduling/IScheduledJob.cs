using System.Threading;
using System.Threading.Tasks;

namespace Mifs.Scheduling
{
    public interface IScheduledJob
    {
        Task Execute(CancellationToken cancellationToken);
    }
}

using System.Threading;
using System.Threading.Tasks;

namespace Mifs
{
    public interface IInitialize 
    {
        Task Initialize(CancellationToken cancellationToken);
    }
}

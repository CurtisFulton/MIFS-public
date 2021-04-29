using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Mifs.MEX.Api
{
    public record ServiceOpParameter(object Value, bool IsString);

    public interface IServiceOpService
    {
        Task<TResult> Execute<TResult>(string serviceOpName, IEnumerable<ServiceOpParameter> parameters, CancellationToken cancellationToken = default);
    }
}

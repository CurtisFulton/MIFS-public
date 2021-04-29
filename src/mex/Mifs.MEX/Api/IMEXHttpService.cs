using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Mifs.MEX.Api
{
    public interface IMEXHttpService
    {
        Task<TResult> PerformAction<TResult>(string actionType, string actionName, string actionData, CancellationToken cancellationToken = default);
        Task<TResult> PerformAction<TResult>(string actionType, string actionName, HttpContent content, CancellationToken cancellationToken = default);
        Task<TResult> ExecuteRequest<TResult>(HttpRequestMessage httpRequest, CancellationToken cancellationToken = default);
    }
}

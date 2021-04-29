using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Mifs.MEX.Api
{
    internal class MEXDateTimeHeadersDelegatingHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("clientdatetime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            return base.SendAsync(request, cancellationToken);
        }
    }
}

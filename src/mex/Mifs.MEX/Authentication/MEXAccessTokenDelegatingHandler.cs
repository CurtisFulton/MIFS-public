using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Mifs.MEX.Authentication
{
    public class MEXAccessTokenDelegatingHandler : DelegatingHandler
    {
        public MEXAccessTokenDelegatingHandler(IMEXAuthenticationService authenticationService)
        {
            this.AuthenticationService = authenticationService;
        }

        private IMEXAuthenticationService AuthenticationService { get; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var accessToken = await this.AuthenticationService.GetAccessToken();
            var accessTokenString = accessToken.ToString();

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenString);

            var response = await base.SendAsync(request, cancellationToken);

            // TODO: Check the response came back not 401

            return response;
        }
    }
}

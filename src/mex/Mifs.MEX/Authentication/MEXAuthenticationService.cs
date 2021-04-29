using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Mifs.MEX.Api;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Mifs.MEX.Authentication
{
    public class MEXAuthenticationService : IMEXAuthenticationService
    {
        public static string HttpClientName { get; } = "MEXAuthenticationClient";

        private static string RefreshTokenCacheKey { get; } = $"{nameof(MEXAuthenticationService)}_RefreshToken";
        private static string AccessTokenCacheKey { get; } = $"{nameof(MEXAuthenticationService)}_AccessToken";

        public MEXAuthenticationService(IHttpClientFactory httpClientFactory,
                                        IMemoryCache memoryCache, 
                                        IOptions<MEXConfiguration> mexConfiguration)
        {
            this.HttpClientFactory = httpClientFactory;
            this.MemoryCache = memoryCache;
            this.MexConfiguration = mexConfiguration.Value;
        }

        private IHttpClientFactory HttpClientFactory { get; }
        private IMemoryCache MemoryCache { get; }
        private MEXConfiguration MexConfiguration { get; }

        public Task<MEXJwtToken> GetAccessToken()
            => this.MemoryCache.GetOrCreateAsync(MEXAuthenticationService.AccessTokenCacheKey, this.RetrieveNewAccessToken);

        private Task<MEXJwtToken> GetRefreshToken()
            => this.MemoryCache.GetOrCreateAsync(MEXAuthenticationService.RefreshTokenCacheKey, this.RetrieveNewRefreshToken);

        private async Task<MEXJwtToken> RetrieveNewAccessToken(ICacheEntry entry)
        {
            var refreshToken = await this.GetRefreshToken();

            // TODO: Wanna clean this up. 
            // Check if a new access token was stored when we requested the refresh token
            if (this.MemoryCache.TryGetValue(MEXAuthenticationService.AccessTokenCacheKey, out MEXJwtToken existingAccessToken))
            {
                return existingAccessToken;
            }

            using var httpClient = this.HttpClientFactory.CreateClient(MEXAuthenticationService.HttpClientName);
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "GenerateAccessToken?useCookies=false");
            httpRequest.Headers.Add("RefreshToken", refreshToken.ToString());

            var response = await httpClient.SendAsync(httpRequest);

            var accessTokenString = response.Headers.First(x => x.Key == "Set-AccessToken").Value.First();
            var accessToken = new MEXJwtToken(accessTokenString);
            entry.AbsoluteExpiration = accessToken.ExpirationDateTime;

            return accessToken;
        }

        private async Task<MEXJwtToken> RetrieveNewRefreshToken(ICacheEntry entry)
        {
            using var httpClient = this.HttpClientFactory.CreateClient(MEXAuthenticationService.HttpClientName);

            var authDetails = this.GetAuthenticationDetails();
            var response = await httpClient.PostAsJsonAsync("RequestRefreshToken", authDetails);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ODataFunctionImportQueryableData>();
            if (!result.FieldValue.Contains("SUCCESS", StringComparison.InvariantCultureIgnoreCase))
            {

                throw new MEXAuthenticationException("Failed to retrieve new Refresh Token.");
            }

            var accessTokenString = response.Headers.First(x => x.Key == "Set-AccessToken").Value.First();
            var accessToken = new MEXJwtToken(accessTokenString);
            this.MemoryCache.Set(MEXAuthenticationService.AccessTokenCacheKey, accessToken, accessToken.ExpirationDateTime);

            var refreshTokenString = response.Headers.First(x => x.Key == "Set-RefreshToken").Value.First();
            var refreshToken = new MEXJwtToken(refreshTokenString);
            entry.AbsoluteExpiration = refreshToken.ExpirationDateTime;

            return refreshToken;
        }

        private AuthenticationDetails GetAuthenticationDetails()
        {
            var userName = this.MexConfiguration.Username;
            var password = this.MexConfiguration.Username;
            var domain = this.MexConfiguration.Domain;

            return new AuthenticationDetails(userName, password, 365)
            {
                DomainName = domain
            };
        }

        private record AuthenticationDetails(string UserName, string Password, int ExpiryInDays)
        {
            public string DomainName { get; init; }
            public bool IsForLicensing { get; init; }
            public bool IsOps { get; init; } 
            public bool UseCookies { get; init; }
        };
    }
}

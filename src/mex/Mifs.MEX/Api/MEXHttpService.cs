using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Mifs.MEX.Api
{
    internal class MEXHttpService : IMEXHttpService
    {
        public static string HttpClientName { get; } = "MEXClient";
        public IHttpClientFactory HttpClientFactory { get; }

        public MEXHttpService(IHttpClientFactory httpClientFactory)
        {
            this.HttpClientFactory = httpClientFactory;
        }

        public async Task<TResult> PerformAction<TResult>(string actionType, string actionName, string actionData, CancellationToken cancellationToken = default)
        {
            var requestUri = new Uri($"API/DataAPI/PerformAction?ActionType={actionType}&ActionName={actionName}&actionData{actionData}", UriKind.Relative);
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, requestUri);

            var result = await this.ExecuteRequest<TResult>(httpRequest, cancellationToken);
            return result;
        }

        public async Task<TResult> PerformAction<TResult>(string actionType, string actionName, HttpContent content, CancellationToken cancellationToken = default)
        {
            var requestUri = new Uri($"API/DataAPI/PerformAction?ActionType={actionType}&ActionName={actionName}", UriKind.Relative);
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = content
            };

            var result = await this.ExecuteRequest<TResult>(httpRequest, cancellationToken);
            return result;
        }

        public async Task<TResult> ExecuteRequest<TResult>(HttpRequestMessage httpRequest, CancellationToken cancellationToken = default)
        {
            using var httpClient = this.HttpClientFactory.CreateClient(HttpClientName);

            var response = await httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseContentRead, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<TResult>(cancellationToken: cancellationToken);

            return result;
        }
    }
}

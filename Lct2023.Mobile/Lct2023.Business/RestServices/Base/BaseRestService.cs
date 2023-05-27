using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataModel.Responses.BaseCms;
using Lct2023.Business.Helpers;
using Newtonsoft.Json;

namespace Lct2023.Business.RestServices.Base
{
    public abstract class BaseRestService
    {
        private readonly HttpClient _httpClient;
        private readonly IRequestAuthenticator _requestAuthenticator;
        private readonly string _basePath;

        protected BaseRestService(HttpClient httpClient, IRequestAuthenticator requestAuthenticator, string basePath)
        {
            _httpClient = httpClient;
            _requestAuthenticator = requestAuthenticator;
            _basePath = basePath;
        }

        [Obsolete("Better inherite from CmsBaseRestService")]
        protected async Task<TResult> CmsExecuteAsync<TResult>(string url, HttpMethod method, CancellationToken token = default) =>
            (await ExecuteAsync<CmsResponse<TResult>>(url, method, token)).Data;

        protected Task<TResult> ExecuteAsync<TResult>(string url, HttpMethod method, CancellationToken token = default)
            => ExecuteInternalAsync<TResult>(url, method, token: token);

        protected Task<TResult> ExecuteAsync<TParam, TResult>(string url, TParam parameter, HttpMethod method, CancellationToken token = default)
            => ExecuteInternalAsync<TResult>(
                url,
                method,
                content: new StringContent(JsonConvert.SerializeObject(parameter), Encoding.UTF8, MediaTypeNames.Application.Json),
                token);

        private async Task<TResult> ExecuteInternalAsync<TResult>(string url, HttpMethod method, HttpContent content = null, CancellationToken token = default)
        {
            using var request = new HttpRequestMessage(method, $"{_basePath}{url}")
            {
                Content = content,
            };

            if (_requestAuthenticator.GetAuthorizationHeader(request.RequestUri.OriginalString) is { } authorizationHeader)
            {
                request.Headers.Authorization = authorizationHeader;
            }

            using var response = await _httpClient.SendAsync(request, token);

            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<TResult>(await response.Content.ReadAsStringAsync());
        }
    }
}

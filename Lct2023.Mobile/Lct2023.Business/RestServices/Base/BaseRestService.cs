using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataModel.Responses.BaseCms;
using Newtonsoft.Json;

namespace Lct2023.Business.RestServices.Base
{
    public abstract class BaseRestService
    {
        private readonly HttpClient _httpClient;
        private readonly UserContext _userContext;

        protected BaseRestService(HttpClient httpClient, UserContext userContext)
        {
            _httpClient = httpClient;
            _userContext = userContext;
        }
        
        protected async Task<TResult> CmsExecuteAsync<TResult>(string url, HttpMethod method, CancellationToken token = default) => (await ExecuteAsync<CmsResponsible<TResult>>(url, method, token)).Data;

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
            using var request = new HttpRequestMessage(method, url)
            {
                Content = content,
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _userContext.AccessToken);
            using var response = await _httpClient.SendAsync(request, token);

            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<TResult>(await response.Content.ReadAsStringAsync());
        }
    }
}
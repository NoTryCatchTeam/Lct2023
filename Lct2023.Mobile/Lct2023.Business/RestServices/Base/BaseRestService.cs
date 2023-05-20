using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataModel.Responses.BaseCms;
using Lct2023.Commons.Extensions;
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
        
        protected async Task<TResult> CmsExecuteAsync<TResult>(string url, HttpMethod method = null, CancellationToken token = default)
        {
            var response = await ExecuteAsync<CmsResponsible<TResult>>(url, method, token);

            if (response.Error != null)
            {
                throw new HttpRequestException(response.Error.Message);
            }

            return response.Data;
        }
        
        protected Task<TResult> ExecuteAsync<TResult>(string url, HttpMethod method = null, CancellationToken token = default) 
            => ExecuteAsync<object, TResult>(url, content: null, method ?? HttpMethod.Get, token);

        protected async Task<TResult> ExecuteAsync<TParam, TResult>(string url, TParam content = default, HttpMethod method = null, CancellationToken token = default)
        {
            using var request = new HttpRequestMessage(method, url)
            {
                Content = content?.Then(c => new StringContent(JsonConvert.SerializeObject(c), Encoding.UTF8, MediaTypeNames.Application.Json)),
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _userContext.AccessToken);
            using var response = await _httpClient.SendAsync(request, token);

            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<TResult>(await response.Content.ReadAsStringAsync());
        }
    }
}
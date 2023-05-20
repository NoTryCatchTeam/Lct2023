using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Lct2023.Business.RestServices.Base
{
    public abstract class BaseRestService
    {
        private readonly HttpClient _httpClient;

        protected BaseRestService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        
        protected async Task<TResult> ExecuteAsync<TResult>(string url, HttpMethod method = null, CancellationToken token = default)
        {
            using var request = new HttpRequestMessage(method ?? HttpMethod.Get, url);
            using var response = await _httpClient.SendAsync(request, token);

            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<TResult>(await response.Content.ReadAsStringAsync());
        }

        protected async Task<TResult> ExecuteAsync<TParam, TResult>(string url, TParam content, HttpMethod method = null, CancellationToken token = default)
        {
            using var request = new HttpRequestMessage(method, url)
            {
                Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, MediaTypeNames.Application.Json),
            };
            using var response = await _httpClient.SendAsync(request, token);

            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<TResult>(await response.Content.ReadAsStringAsync());
        }
    }
}
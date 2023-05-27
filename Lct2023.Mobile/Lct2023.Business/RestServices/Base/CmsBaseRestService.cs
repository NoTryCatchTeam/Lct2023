using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DataModel.Responses.BaseCms;
using Lct2023.Business.Helpers;

namespace Lct2023.Business.RestServices.Base;

public class CmsBaseRestService : BaseRestService, ICmsBaseRestService
{
    public CmsBaseRestService(HttpClient httpClient, IRequestAuthenticator requestAuthenticator, string basePath)
        : base(httpClient, requestAuthenticator, basePath)
    {
    }
    
    protected async Task<TResult> CmsExecuteAsync<TResult>(string url, HttpMethod method, CancellationToken token = default) =>
        (await ExecuteAsync<CmsResponse<TResult>>(url, method, token)).Data;
    
    protected Task<CmsResponse<TResult>> CmsPaginationExecuteAsync<TResult>(string url, int start, int limit, HttpMethod method, CancellationToken token = default)
        => ExecuteAsync<CmsResponse<TResult>>($"{url}&pagination[start]={start}&pagination[limit]={limit}", method, token);
}
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DataModel.Definitions.Enums;
using DataModel.Responses.BaseCms;
using Lct2023.Business.Helpers;
using Lct2023.Commons.Extensions;

namespace Lct2023.Business.RestServices.Base;

public class CmsBaseRestService : BaseRestService, ICmsBaseRestService
{
    public CmsBaseRestService(HttpClient httpClient, IRequestAuthenticator requestAuthenticator, string basePath)
        : base(httpClient, requestAuthenticator, basePath)
    {
    }
    
    protected async Task<TResult> CmsExecuteAsync<TResult>(string url, HttpMethod method, CancellationToken token = default) =>
        (await ExecuteAsync<CmsResponse<TResult>>(url, method, token)).Data;
    
    protected Task<CmsResponse<TResult>> CmsPaginationExecuteAsync<TResult>(string url, string sortingField, SortingType sortingType, int start, int limit, HttpMethod method, CancellationToken token = default)
        => CmsPaginationExecuteAsync<TResult>($"{url}&sort[0]={sortingField}%3A{sortingType.GetEnumMemberValue()}", start, limit, method, token);
    protected Task<CmsResponse<TResult>> CmsPaginationExecuteAsync<TResult>(string url, IEnumerable<(string sortingField, SortingType sortingType)> sorting, int start, int limit, HttpMethod method, CancellationToken token = default)
        => CmsPaginationExecuteAsync<TResult>($"{url}&{string.Join('&', sorting.Select((s, i) => $"sort[{i}]={s.sortingField}%3A{s.sortingType.GetEnumMemberValue()}"))}", start, limit, method, token);
    
    protected Task<CmsResponse<TResult>> CmsPaginationExecuteAsync<TResult>(string url, int start, int limit, HttpMethod method, CancellationToken token = default)
        => ExecuteAsync<CmsResponse<TResult>>($"{url}&pagination[start]={start}&pagination[limit]={limit}", method, token);
}
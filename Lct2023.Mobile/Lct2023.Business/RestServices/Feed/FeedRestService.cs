using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DataModel.Definitions.Enums;
using DataModel.Responses.BaseCms;
using DataModel.Responses.Feed;
using Lct2023.Business.Helpers;
using Lct2023.Business.RestServices.Base;
using Lct2023.Commons.Extensions;

namespace Lct2023.Business.RestServices.Feed;

public class FeedRestService : CmsBaseRestService, IFeedRestService
{
    public FeedRestService(HttpClient httpClient, IRequestAuthenticator requestAuthenticator)
        : base(httpClient, requestAuthenticator, BusinessInit.Instance.BaseCmsPath)
    {
    }
    
    public Task<CmsResponse<IEnumerable<CmsItemResponse<ArticleResponse>>>> GetArticlesPaginationAsync(int start, int limit, string search, IEnumerable<(string field, string[] values)> filters, CancellationToken token) =>
        CmsPaginationExecuteAsync<IEnumerable<CmsItemResponse<ArticleResponse>>>(
            @$"articles?fields[0]=title&fields[0]=publishedAt&fields[0]=text&fields[0]=link&populate[rubric][fields][0]=name&populate[art_categories][fields][0]=name&populate[art_categories][fields][0]=displayName&populate[cover][fields][0]=url&populate[cover][fields][0]=ext&populate[cover][fields][0]=mime&populate[cover][fields][0]=name"
            + $"{search?.ThenIfNotNullOrWhiteSpace(s => $"&filters[$and][0][text][$containsi]={s}")}"
            + $"{filters?.ThenIfNotEmpty(fs => $"&{string.Join('&', fs.SelectMany(f => f.values?.Select(v => $"filters[$or][1]{f.field}[$eq]={v}")))}")}",
            "publishedAt",
            SortingType.Desc,
            start,
            limit,
            HttpMethod.Get,
            token);
    
    public Task<IEnumerable<CmsItemResponse<RubricResponse>>> GetRubricsAsync(CancellationToken token) =>
        CmsExecuteAsync<IEnumerable<CmsItemResponse<RubricResponse>>>("rubrics", HttpMethod.Get, token);
}
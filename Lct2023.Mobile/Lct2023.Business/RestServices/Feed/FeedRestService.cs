using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DataModel.Responses.BaseCms;
using DataModel.Responses.Feed;
using Lct2023.Business.Helpers;
using Lct2023.Business.RestServices.Base;

namespace Lct2023.Business.RestServices.Feed;

public class FeedRestService : CmsBaseRestService, IFeedRestService
{
    public FeedRestService(HttpClient httpClient, IRequestAuthenticator requestAuthenticator)
        : base(httpClient, requestAuthenticator, BusinessInit.Instance.BaseCmsPath)
    {
    }
    
    public Task<CmsResponse<IEnumerable<CmsItemResponse<ArticleResponse>>>> GetArticlesPaginationAsync(int start, int limit, CancellationToken token) =>
        CmsPaginationExecuteAsync<IEnumerable<CmsItemResponse<ArticleResponse>>>("articles?fields[0]=title&fields[0]=publishedAt&fields[0]=text&fields[0]=link&populate[rubric][fields][0]=name&populate[art_categories][fields][0]=name&populate[art_categories][fields][0]=displayName&populate[cover][fields][0]=url&populate[cover][fields][0]=ext&populate[cover][fields][0]=mime&populate[cover][fields][0]=name", start, limit, HttpMethod.Get, token);
    
    public Task<IEnumerable<CmsItemResponse<RubricResponse>>> GetRubricsAsync(CancellationToken token) =>
        CmsExecuteAsync<IEnumerable<CmsItemResponse<RubricResponse>>>("rubrics", HttpMethod.Get, token);
}
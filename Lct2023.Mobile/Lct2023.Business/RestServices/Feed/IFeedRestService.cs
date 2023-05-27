using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataModel.Responses.BaseCms;
using DataModel.Responses.Feed;

namespace Lct2023.Business.RestServices.Feed;

public interface IFeedRestService
{
    Task<CmsResponse<IEnumerable<CmsItemResponse<ArticleResponse>>>> GetArticlesPaginationAsync(int start, int limit, CancellationToken token);

    Task<IEnumerable<CmsItemResponse<RubricResponse>>> GetRubricsAsync(CancellationToken token);
}
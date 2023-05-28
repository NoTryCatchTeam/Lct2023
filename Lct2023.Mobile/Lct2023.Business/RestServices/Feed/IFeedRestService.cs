using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataModel.Responses.BaseCms;
using DataModel.Responses.Feed;

namespace Lct2023.Business.RestServices.Feed;

public interface IFeedRestService
{
    Task<CmsResponse<IEnumerable<CmsItemResponse<ArticleResponse>>>> GetArticlesPaginationAsync(int start, int limit, string search, IEnumerable<(string field, string[] values)> filters, CancellationToken token);

    Task<IEnumerable<CmsItemResponse<RubricResponse>>> GetRubricsAsync(CancellationToken token);
}
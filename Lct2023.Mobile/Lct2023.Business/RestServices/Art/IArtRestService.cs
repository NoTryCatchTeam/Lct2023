using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataModel.Responses.Art;
using DataModel.Responses.BaseCms;

namespace Lct2023.Business.RestServices.Art;

public interface IArtRestService
{
    Task<IEnumerable<CmsItemResponse<StreamResponse>>> GetStreamsAsync(CancellationToken token);

    Task<IEnumerable<CmsItemResponse<ArtCategoryResponse>>> GetArtCategoriesAsync(CancellationToken token);
}
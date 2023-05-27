using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DataModel.Responses.Art;
using DataModel.Responses.BaseCms;
using Lct2023.Business.Helpers;
using Lct2023.Business.RestServices.Base;

namespace Lct2023.Business.RestServices.Art;

public class ArtRestService : BaseRestService, IArtRestService
{
    public ArtRestService(HttpClient httpClient, IRequestAuthenticator requestAuthenticator)
        : base(httpClient, requestAuthenticator, BusinessInit.Instance.BaseCmsPath)
    {
    }

    public Task<IEnumerable<CmsItemResponse<StreamResponse>>> GetStreamsAsync(CancellationToken token) =>
        CmsExecuteAsync<IEnumerable<CmsItemResponse<StreamResponse>>>("streams?populate[art_category][fields][0]=name&populate[art_category][fields][0]=displayName", HttpMethod.Get, token);

    public Task<IEnumerable<CmsItemResponse<ArtCategoryResponse>>> GetArtCategoriesAsync(CancellationToken token) =>
        CmsExecuteAsync<IEnumerable<CmsItemResponse<ArtCategoryResponse>>>("art-categories", HttpMethod.Get, token);
}
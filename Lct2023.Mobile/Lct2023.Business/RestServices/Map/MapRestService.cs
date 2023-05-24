using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DataModel.Responses.BaseCms;
using DataModel.Responses.Map;
using Lct2023.Business.Helpers;
using Lct2023.Business.RestServices.Base;

namespace Lct2023.Business.RestServices.Map
{
    public class MapRestService : BaseRestService, IMapRestService
    {
        public MapRestService(HttpClient httpClient, IRequestAuthenticator requestAuthenticator)
            : base(httpClient, requestAuthenticator, BusinessInit.Instance.BaseCmsPath)
        {
        }

        public Task<IEnumerable<CmsItemResponse<SchoolLocationResponse>>> GetSchoolsLocationAsync(CancellationToken token) =>
            CmsExecuteAsync<IEnumerable<CmsItemResponse<SchoolLocationResponse>>>("locations", HttpMethod.Get, token);
    }
}

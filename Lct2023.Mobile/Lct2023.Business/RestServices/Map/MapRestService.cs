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
            CmsExecuteAsync<IEnumerable<CmsItemResponse<SchoolLocationResponse>>>("locations?populate[district][fields][0]=area", HttpMethod.Get, token);

        public Task<IEnumerable<CmsItemResponse<EventItemResponse>>> GetEventsAsync(CancellationToken token) =>
            CmsExecuteAsync<IEnumerable<CmsItemResponse<EventItemResponse>>>("events?fields[0]=name&fields[0]=description&fields[0]=link&populate[place][fields][0]=lat&populate[place][fields][0]=lon&populate[place][fields][0]=address&fields[0]=ticketLink&fields[0]=createdAt", HttpMethod.Get, token);
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DataModel.Responses.BaseCms;
using DataModel.Responses.Map;
using Lct2023.Business.Helpers;
using Lct2023.Business.RestServices.Base;
using Lct2023.Commons.Extensions;

namespace Lct2023.Business.RestServices.Map
{
    public class MapRestService : CmsBaseRestService, IMapRestService
    {
        public MapRestService(HttpClient httpClient, IRequestAuthenticator requestAuthenticator)
            : base(httpClient, requestAuthenticator, BusinessInit.Instance.BaseCmsPath)
        {
        }
        
        public Task<CmsResponse<IEnumerable<CmsItemResponse<DistrictResponse>>>> GetDistrictsPaginationAsync(int start, int limit, CancellationToken token) =>
            CmsPaginationExecuteAsync<IEnumerable<CmsItemResponse<DistrictResponse>>>("districts?", start, limit, HttpMethod.Get, token);

        public Task<CmsResponse<IEnumerable<CmsItemResponse<SchoolLocationResponse>>>> GetSchoolsLocationPaginationAsync(int start, int limit, CancellationToken token, IEnumerable<string> artDirections = null) =>
            CmsPaginationExecuteAsync<IEnumerable<CmsItemResponse<SchoolLocationResponse>>>("locations?populate[district][fields][0]=area&populate[district][fields][0]=district&populate[streams][fields][0]=name"
                + $"{artDirections?.ThenIfNotEmpty(aDs => string.Concat(aDs.Select(aD => $"&filters[$or][0][streams][art_category][displayName][$eq]={aD}")))}", start, limit, HttpMethod.Get, token);

        public Task<CmsResponse<IEnumerable<CmsItemResponse<EventItemResponse>>>> GetEventsPaginationAsync(int start, int limit, CancellationToken token) =>
            CmsPaginationExecuteAsync<IEnumerable<CmsItemResponse<EventItemResponse>>>("events?fields[0]=name&fields[0]=description&fields[0]=link&populate[place][fields][0]=lat&populate[place][fields][0]=lon&populate[place][fields][0]=address&fields[0]=ticketLink&fields[0]=createdAt&populate[streams][fields][0]=name&populate[place][populate][0]=district", start, limit, HttpMethod.Get, token);
        
        public Task<IEnumerable<CmsItemResponse<EventItemResponse>>> GetMainEventsAsync(CancellationToken token) =>
            CmsExecuteAsync<IEnumerable<CmsItemResponse<EventItemResponse>>>("events?[populate]=*&sort[eventDate]=ASC", HttpMethod.Get, token);
    }
}

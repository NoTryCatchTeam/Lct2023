using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using DataModel.Responses.BaseCms;
using DataModel.Responses.Map;
using Lct2023.Business.RestServices.Base;

namespace Lct2023.Business.RestServices.Map
{
    public class MapRestService : BaseRestService, IMapRestService
    {
        public MapRestService(HttpClient httpClient, UserContext userContext)
            : base(httpClient, userContext)
        {
        }

        public Task<IEnumerable<CmsItemResponse<SchoolLocationResponse>>> GetSchoolsLocationAsync() => CmsExecuteAsync<IEnumerable<CmsItemResponse<SchoolLocationResponse>>>("/locations");
    }
}
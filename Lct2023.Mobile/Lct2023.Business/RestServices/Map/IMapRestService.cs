using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataModel.Responses.BaseCms;
using DataModel.Responses.Map;
using Lct2023.Business.RestServices.Base;

namespace Lct2023.Business.RestServices.Map
{
    public interface IMapRestService : ICmsBaseRestService
    {
        Task<CmsResponse<IEnumerable<CmsItemResponse<DistrictResponse>>>> GetDistrictsPaginationAsync(int start, int limit, CancellationToken token);
        
        Task<CmsResponse<IEnumerable<CmsItemResponse<SchoolLocationResponse>>>> GetSchoolsLocationPaginationAsync(int start, int limit, CancellationToken token, IEnumerable<string> artDirections = null);
        
        Task<CmsResponse<IEnumerable<CmsItemResponse<EventItemResponse>>>> GetEventsPaginationAsync(int start, int limit, CancellationToken token);

        Task<IEnumerable<CmsItemResponse<EventItemResponse>>> GetMainEventsAsync(CancellationToken token);
    }
}

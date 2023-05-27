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
        
        Task<IEnumerable<CmsItemResponse<SchoolLocationResponse>>> GetSchoolsLocationAsync(CancellationToken token);
        
        Task<IEnumerable<CmsItemResponse<EventItemResponse>>> GetEventsAsync(CancellationToken token);
    }
}

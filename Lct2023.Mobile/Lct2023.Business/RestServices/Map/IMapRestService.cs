using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataModel.Responses.BaseCms;
using DataModel.Responses.Map;

namespace Lct2023.Business.RestServices.Map
{
    public interface IMapRestService
    {
        Task<IEnumerable<CmsItemResponse<SchoolLocationResponse>>> GetSchoolsLocationAsync(CancellationToken token);
        
        Task<IEnumerable<CmsItemResponse<EventItemResponse>>> GetEventsAsync(CancellationToken token);
    }
}

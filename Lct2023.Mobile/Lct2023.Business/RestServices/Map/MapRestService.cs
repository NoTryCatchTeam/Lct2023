using System.Net.Http;
using System.Threading.Tasks;
using Lct2023.Business.RestServices.Base;

namespace Lct2023.Business.RestServices.Map
{
    public class MapRestService : BaseRestService, IMapRestService
    {
        public MapRestService(HttpClient httpClient)
            : base(httpClient)
        {
        }

        public Task<object> GetSchoolsLocationAsync() => ExecuteAsync<object>("/locations");
    }
}
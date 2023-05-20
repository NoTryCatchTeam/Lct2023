using System.Threading.Tasks;

namespace Lct2023.Business.RestServices.Map
{
    public interface IMapRestService
    {
        Task<object> GetSchoolsLocationAsync();
    }
}
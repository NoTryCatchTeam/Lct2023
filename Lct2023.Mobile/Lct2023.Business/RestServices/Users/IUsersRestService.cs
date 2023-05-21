using System.Threading;
using System.Threading.Tasks;
using DataModel.Responses.Users;

namespace Lct2023.Business.RestServices.Users;

public interface IUsersRestService
{
    Task<UserItemResponse> GetSelfAsync(CancellationToken token);
}

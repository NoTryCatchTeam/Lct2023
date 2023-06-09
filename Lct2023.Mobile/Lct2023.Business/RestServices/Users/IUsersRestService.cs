using System.Threading;
using System.Threading.Tasks;
using DataModel.Responses.Users;

namespace Lct2023.Business.RestServices.Users;

public interface IUsersRestService
{
    Task<UserItemResponse> GetSelfAsync(CancellationToken token);

    Task<int> GetRatingAsync(CancellationToken token);

    Task<int> UpdateRatingAsync(int increment, CancellationToken token);
}

using System.Threading;
using System.Threading.Tasks;
using DataModel.Requests.Auth;
using DataModel.Responses.Auth;

namespace Lct2023.Business.RestServices.Auth;

public interface IAuthRestService
{
    Task<AuthSuccessResponse> SignInBasicAsync(SignInBasicRequest request, CancellationToken token);

    Task<AuthSuccessResponse> SignUpAsync(CreateUserRequest request, CancellationToken token);
}

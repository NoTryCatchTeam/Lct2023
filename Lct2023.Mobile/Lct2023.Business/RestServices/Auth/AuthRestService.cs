using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DataModel.Requests.Auth;
using DataModel.Responses.Auth;
using Lct2023.Business.Helpers;
using Lct2023.Business.RestServices.Base;

namespace Lct2023.Business.RestServices.Auth;

public class AuthRestService : BaseRestService, IAuthRestService
{
    public AuthRestService(HttpClient httpClient, IRequestAuthenticator requestAuthenticator)
        : base(httpClient, requestAuthenticator)
    {
    }

    public Task<AuthSuccessResponse> SignInBasicAsync(SignInBasicRequest request, CancellationToken token) =>
        ExecuteAsync<SignInBasicRequest, AuthSuccessResponse>("auth/sign-in/basic", request, HttpMethod.Post, token);

    public Task<AuthSuccessResponse> SignUpAsync(CreateUserRequest request, CancellationToken token) =>
        ExecuteAsync<CreateUserRequest, AuthSuccessResponse>("auth/sign-up", request, HttpMethod.Post, token);
}

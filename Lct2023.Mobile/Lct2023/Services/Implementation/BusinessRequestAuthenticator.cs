using System.Net.Http.Headers;
using Lct2023.Business.Helpers;
using Lct2023.Definitions;

namespace Lct2023.Services.Implementation;

public class BusinessRequestAuthenticator : IRequestAuthenticator
{
    private readonly IUserContext _userContext;

    public BusinessRequestAuthenticator(IUserContext userContext)
    {
        _userContext = userContext;
    }

    public string GetAccessToken() =>
        _userContext.User.AccessToken;

    public AuthenticationHeaderValue GetAuthorizationHeader(string path) =>
        path.StartsWith("auth") ?
            null :
            new AuthenticationHeaderValue("Bearer", _userContext.User.AccessToken);
}

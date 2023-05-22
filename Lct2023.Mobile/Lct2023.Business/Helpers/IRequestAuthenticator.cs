using System.Net.Http.Headers;

namespace Lct2023.Business.Helpers;

public interface IRequestAuthenticator
{
    AuthenticationHeaderValue GetAuthorizationHeader(string path);
}

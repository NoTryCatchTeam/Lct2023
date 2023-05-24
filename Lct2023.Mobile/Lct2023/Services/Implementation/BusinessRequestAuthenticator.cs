using System.Net.Http.Headers;
using Lct2023.Business.Helpers;
using Lct2023.Definitions;
using Lct2023.Definitions.Constants;
using Microsoft.Extensions.Configuration;

namespace Lct2023.Services.Implementation;

public class BusinessRequestAuthenticator : IRequestAuthenticator
{
    private readonly IConfiguration _configuration;
    private readonly IUserContext _userContext;

    public BusinessRequestAuthenticator(IConfiguration configuration, IUserContext userContext)
    {
        _configuration = configuration;
        _userContext = userContext;
    }

    public AuthenticationHeaderValue GetAuthorizationHeader(string path)
    {
        if (path.StartsWith(_configuration.GetValue<string>(ConfigurationConstants.AppSettings.CMS_PATH)))
        {
            return new AuthenticationHeaderValue("Bearer", _configuration.GetValue<string>(ConfigurationConstants.Secrets.CMS_TOKEN));
        }

        return path.StartsWith($"{_configuration.GetValue<string>(ConfigurationConstants.AppSettings.API_PATH)}/auth") ?
            null :
            new AuthenticationHeaderValue("Bearer", _userContext.User.AccessToken);
    }
}

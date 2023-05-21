using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DataModel.Requests.Auth;
using Lct2023.Business.RestServices.Auth;
using Lct2023.Business.RestServices.Users;
using Lct2023.Definitions.Constants;
using Lct2023.Definitions.Dtos;
using Lct2023.Definitions.Internals;
using Microsoft.Extensions.Configuration;
using Xamarin.Essentials;

namespace Lct2023.Services.Implementation;

public class UserService : IUserService
{
    private readonly IAuthRestService _authRestService;
    private readonly IUsersRestService _usersRestService;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public UserService(
        IAuthRestService authRestService,
        IUsersRestService usersRestService,
        IUserContext userContext,
        IMapper mapper,
        IConfiguration configuration)
    {
        _authRestService = authRestService;
        _usersRestService = usersRestService;
        _userContext = userContext;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task SignInViaSocialAsync(CancellationToken token)
    {
        var callback = Regex.Match(AppInfo.PackageName, "^.*[.].*[.](.*)$").Groups[1].Value;

        var authResult = await WebAuthenticator.AuthenticateAsync(
            new WebAuthenticatorOptions
            {
                Url = new Uri($"{_configuration.GetValue<string>(ConfigurationConstants.AppSettings.HOST)}auth/sign-in/vk"),
                CallbackUrl = new Uri($"{callback}://"),
            }
        );

        if (string.IsNullOrEmpty(authResult.AccessToken) || string.IsNullOrEmpty(authResult.RefreshToken))
        {
            throw new Exception("Couldn't sign-in use via social");
        }

        await RequestAndStoreUserInfoAsync(authResult.AccessToken, authResult.RefreshToken, token);
    }

    public async Task SignInAsync(string login, string password, CancellationToken token)
    {
        var authResponse = await _authRestService.SignInBasicAsync(
            new SignInBasicRequest
            {
                Login = login,
                Password = password,
            },
            token);

        await RequestAndStoreUserInfoAsync(authResponse.AccessToken, authResponse.RefreshToken, token);
    }

    public async Task SignUpAsync(CreateUserDto dto, CancellationToken token)
    {
        var authResponse = await _authRestService.SignUpAsync(_mapper.Map<CreateUserRequest>(dto), token);

        await RequestAndStoreUserInfoAsync(authResponse.AccessToken, authResponse.RefreshToken, token);
    }

    public Task SignOutAsync(CancellationToken token)
    {
        _userContext.Reset();

        return Task.CompletedTask;
    }

    private async Task RequestAndStoreUserInfoAsync(string accessToken, string refreshToken, CancellationToken token)
    {
        var user = new User
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };

        // Store temp user to request info
        await _userContext.StoreAsync(user);

        try
        {
            var userResponse = await _usersRestService.GetSelfAsync(token);

            user = _mapper.Map<User>(userResponse);
            user.AccessToken = accessToken;
            user.RefreshToken = refreshToken;

            await _userContext.StoreAsync(user);
        }
        catch
        {
            _userContext.Reset();

            throw;
        }
    }
}

using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DataModel.Requests.Auth;
using Lct2023.Business.RestServices.Auth;
using Lct2023.Business.RestServices.Users;
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

    public UserService(
        IAuthRestService authRestService,
        IUsersRestService usersRestService,
        IUserContext userContext,
        IMapper mapper)
    {
        _authRestService = authRestService;
        _usersRestService = usersRestService;
        _userContext = userContext;
        _mapper = mapper;
    }

    public async Task SignInViaSocialAsync(CancellationToken token)
    {
        var callback = Regex.Match(AppInfo.PackageName, "^.*[.].*[.](.*)$").Groups[1].Value;

        var authResult = await WebAuthenticator.AuthenticateAsync(
            new WebAuthenticatorOptions
            {
                Url = new Uri(
                    $"http://45.9.27.2:8080/Auth/sign-in/vk"),
                CallbackUrl = new Uri($"{callback}://"),
            }
        );

        if (string.IsNullOrEmpty(authResult.AccessToken) || string.IsNullOrEmpty(authResult.RefreshToken))
        {
            throw new Exception("Couldn't sign-in use via social");
        }

        await RequestAndStoreUserInfoAsync(authResult.AccessToken, authResult.RefreshToken, token);
    }

    public async Task SignInAsync(string email, string password, CancellationToken token)
    {
        var authResponse = await _authRestService.SignInBasicAsync(
            new SignInBasicRequest
            {
                Login = email,
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

    public async Task UpdateRatingAsync(CancellationToken token)
    {
        var user = _userContext.User;

        user.Rating = await _usersRestService.GetRatingAsync(token);

        await _userContext.StoreAsync(user);
    }

    public async Task UpdateRatingAsync(int increment, CancellationToken token)
    {
        var user = _userContext.User;

        user.Rating = await _usersRestService.UpdateRatingAsync(increment, token);

        await _userContext.StoreAsync(user);
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

            user.Rating = await _usersRestService.GetRatingAsync(token);

            await _userContext.StoreAsync(user);
        }
        catch
        {
            _userContext.Reset();

            throw;
        }
    }
}

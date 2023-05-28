using AutoMapper;
using Lct2023.Api.Dal;
using Lct2023.Api.Definitions.Dto;
using Lct2023.Api.Definitions.Entities;
using Lct2023.Api.Definitions.Exceptions;
using Lct2023.Api.Definitions.Identity;
using Lct2023.Api.Definitions.Types;
using Lct2023.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Lct2023.Api.Services;

public class AuthService : IAuthService
{
    private readonly SignInManager<ExtendedIdentityUser> _signInManager;
    private readonly IAuthTokensService _authTokensService;
    private readonly NpgsqlDbContext _dbContext;
    private readonly IMapper _mapper;

    public AuthService(
        SignInManager<ExtendedIdentityUser> signInManager,
        IAuthTokensService authTokensService,
        NpgsqlDbContext dbContext,
        IMapper mapper)
    {
        _signInManager = signInManager;
        _authTokensService = authTokensService;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<UserDto> SignUpAsync(CreateUserDto data)
    {
        if (await _signInManager.UserManager.FindByEmailAsync(data.Email) is { })
        {
            throw new UserException(UserExceptionType.UserExists);
        }

        var result = await _signInManager.UserManager.CreateAsync(_mapper.Map<ExtendedIdentityUser>(data), data.Password);

        if (!result.Succeeded)
        {
            throw new BusinessException($"Couldn't create user: {string.Join(",", result.Errors.Select(x => x.Description).ToList())}");
        }

        return _mapper.Map<UserDto>(await _signInManager.UserManager.FindByEmailAsync(data.Email));
    }

    public async Task<UserDto> SignUpViaSocialAsync(CreateUserViaSocialDto dto)
    {
        if (await _signInManager.UserManager.FindByEmailAsync(dto.Email) is { })
        {
            throw new UserException(UserExceptionType.UserExists);
        }

        var newUser = _mapper.Map<ExtendedIdentityUser>(dto);
        var result = await _signInManager.UserManager.CreateAsync(newUser);

        if (!result.Succeeded)
        {
            throw new BusinessException($"Couldn't create user: {string.Join(",", result.Errors.Select(x => x.Description).ToList())}");
        }

        var externalLoginResult = await _signInManager.UserManager.AddLoginAsync(newUser, new UserLoginInfo(dto.LoginProvider, dto.LoginProviderKey, null));

        if (!externalLoginResult.Succeeded)
        {
            throw new BusinessException($"Couldn't create user: {string.Join(",", externalLoginResult.Errors.Select(x => x.Description).ToList())}");
        }

        return _mapper.Map<UserDto>(await _signInManager.UserManager.FindByEmailAsync(dto.Email));
    }

    public async Task<AuthSuccessDto> SignInAsync(string email, string password)
    {
        if (await _signInManager.UserManager.FindByEmailAsync(email) is not { } user)
        {
            throw new UserException(UserExceptionType.UserDoNotExists);
        }

        if (!(await _signInManager.CheckPasswordSignInAsync(user, password, false)).Succeeded)
        {
            throw new OauthException("Couldn't sign in user: wrong password");
        }

        var userRefreshToken = new UserRefreshToken
        {
            User = user,
        };

        (userRefreshToken.Token, userRefreshToken.ExpiresAt) = _authTokensService.CreateRefreshToken();

        await _dbContext.UserRefreshTokens.AddAsync(userRefreshToken);
        await _dbContext.SaveChangesAsync();

        return new AuthSuccessDto
        {
            AccessToken = _authTokensService.CreateAccessToken(user.Id),
            RefreshToken = userRefreshToken.Token,
        };
    }

    public async Task<AuthSuccessDto> SignInViaSocialAsync(string login, string loginProvider)
    {
        if (await _signInManager.UserManager.FindByNameAsync(login) is not { } user)
        {
            throw new UserException(UserExceptionType.UserDoNotExists);
        }

        if (string.IsNullOrEmpty(user.PasswordHash) &&
            !(await _signInManager.UserManager.GetLoginsAsync(user)).Any())
        {
            throw new BusinessException($"Couldn't sign in user: no {loginProvider} sign up");
        }

        var userRefreshToken = new UserRefreshToken
        {
            User = user,
        };

        (userRefreshToken.Token, userRefreshToken.ExpiresAt) = _authTokensService.CreateRefreshToken();

        await _dbContext.UserRefreshTokens.AddAsync(userRefreshToken);
        await _dbContext.SaveChangesAsync();

        return new AuthSuccessDto
        {
            AccessToken = _authTokensService.CreateAccessToken(user.Id),
            RefreshToken = userRefreshToken.Token,
        };
    }

    public async Task<AuthSuccessDto> RefreshAsync(RefreshTokensDto data)
    {
        if (await _signInManager.UserManager.FindByIdAsync(data.UserId.ToString()) is not { })
        {
            throw new UserException(UserExceptionType.UserDoNotExists);
        }

        if (!_authTokensService.ValidateAccessToken(data.AccessToken))
        {
            throw new OauthException("Invalid access token provided");
        }

        if (await _dbContext.UserRefreshTokens.FirstOrDefaultAsync(x => x.User.Id == data.UserId && x.Token == data.RefreshToken) is not { } refreshTokenEntity)
        {
            throw new OauthException("Invalid refresh token provided");
        }

        if (refreshTokenEntity.ExpiresAt <= DateTimeOffset.Now)
        {
            throw new OauthException("Refresh token expired");
        }

        (refreshTokenEntity.Token, refreshTokenEntity.ExpiresAt) = _authTokensService.CreateRefreshToken();

        _dbContext.UserRefreshTokens.Update(refreshTokenEntity);

        await _dbContext.SaveChangesAsync();

        return new AuthSuccessDto
        {
            AccessToken = _authTokensService.CreateAccessToken(data.UserId),
            RefreshToken = refreshTokenEntity.Token,
        };
    }

    public async Task<string> CreateUserNameAsync(CreateUserNameDto data)
    {
        var userNameEmail = data.Email.Split("@", StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        var count = await _dbContext.Users.Where(x=> x.Email.StartsWith(userNameEmail +'@')).CountAsync(); 
        return userNameEmail + (count+1).ToString();
    }
}

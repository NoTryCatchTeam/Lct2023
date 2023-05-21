using Lct2023.Api.Definitions.Dto;

namespace Lct2023.Api.Services.Interfaces;

public interface IAuthService
{
    Task<UserDto> SignUpAsync(CreateUserDto data);

    Task<UserDto> SignUpViaSocialAsync(CreateUserViaSocialDto dto);

    Task<AuthSuccessDto> SignInAsync(string login, string password);

    Task<AuthSuccessDto> SignInViaSocialAsync(string login, string loginProvider);

    Task<AuthSuccessDto> RefreshAsync(RefreshTokensDto data);
}

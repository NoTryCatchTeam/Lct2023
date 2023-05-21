using Lct2023.Api.Definitions.Dto;

namespace Lct2023.Api.Services.Interfaces;

public interface IUserService
{
    Task<UserDto> GetByEmailAsync(string email);
}

using AutoMapper;
using AutoMapper.QueryableExtensions;
using Lct2023.Api.Definitions.Dto;
using Lct2023.Api.Definitions.Identity;
using Lct2023.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Lct2023.Api.Services;

public class UserService : IUserService
{
    private readonly SignInManager<ExtendedIdentityUser> _signInManager;
    private readonly IMapper _mapper;

    public UserService(SignInManager<ExtendedIdentityUser> signInManager, IMapper mapper)
    {
        _signInManager = signInManager;
        _mapper = mapper;
    }

    public Task<UserDto> GetByEmailAsync(string email) =>
        _signInManager.UserManager.Users.ProjectTo<UserDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(x => x.Email == email);
}

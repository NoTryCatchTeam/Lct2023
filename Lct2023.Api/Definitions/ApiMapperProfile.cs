using AutoMapper;
using DataModel.Requests.Auth;
using DataModel.Responses.Auth;
using DataModel.Responses.Users;
using Lct2023.Api.Definitions.Dto;
using Lct2023.Api.Definitions.Identity;

namespace Lct2023.Api.Definitions;

public class ApiMapperProfile : Profile
{
    public ApiMapperProfile()
    {
        MapAuth();
        MapUsers();
    }

    private void MapAuth()
    {
        CreateMap<AuthSuccessDto, AuthSuccessResponse>();
        CreateMap<RefreshTokensRequest, RefreshTokensDto>();
    }

    private void MapUsers()
    {
        CreateMap<CreateUserRequest, CreateUserDto>()
            .ForMember(id => id.UserName, expr => expr.Ignore());
        CreateMap<UserDto, UserItemResponse>();
        CreateMap<ExtendedIdentityUser, UserItemResponse>();
        CreateMap<ExtendedIdentityUser, UserDto>();

        CreateMap<CreateUserDto, ExtendedIdentityUser>()
            .ForMember(id => id.UserName, expr => expr.MapFrom(dto => dto.UserName))
            .ForMember(id => id.Id, expr => expr.Ignore())
            .ForMember(id => id.BirthDate, expr => expr.MapFrom(dto => dto.BirthDate))
            .ForMember(id => id.NormalizedUserName, expr => expr.Ignore())
            .ForMember(id => id.NormalizedEmail, expr => expr.Ignore())
            .ForMember(id => id.EmailConfirmed, expr => expr.Ignore())
            .ForMember(id => id.PasswordHash, expr => expr.Ignore())
            .ForMember(id => id.SecurityStamp, expr => expr.Ignore())
            .ForMember(id => id.ConcurrencyStamp, expr => expr.Ignore())
            .ForMember(id => id.PhoneNumber, expr => expr.Ignore())
            .ForMember(id => id.PhoneNumberConfirmed, expr => expr.Ignore())
            .ForMember(id => id.TwoFactorEnabled, expr => expr.Ignore())
            .ForMember(id => id.LockoutEnd, expr => expr.Ignore())
            .ForMember(id => id.LockoutEnabled, expr => expr.Ignore())
            .ForMember(id => id.AccessFailedCount, expr => expr.Ignore())
            .ForMember(id => id.CreatedAt, expr => expr.Ignore())
            .ForMember(id => id.UpdatedAt, expr => expr.Ignore())
            .ForMember(id => id.RefreshTokens, expr => expr.Ignore())
            .ForMember(id => id.PhotoUrl, expr => expr.MapFrom(dto => dto.Photo))
            .ForMember(id => id.UserInfo, expr => expr.Ignore())
            ;

        CreateMap<CreateUserViaSocialDto, ExtendedIdentityUser>()
            .ForMember(id => id.UserName, expr => expr.MapFrom(dto => dto.UserName))
            .ForMember(id => id.PhotoUrl, expr => expr.MapFrom(dto => dto.PhotoUrl))
            .ForMember(id => id.BirthDate, expr => expr.MapFrom(dto => dto.BirthDate))
            .ForMember(id => id.Id, expr => expr.Ignore())
            .ForMember(id => id.NormalizedUserName, expr => expr.Ignore())
            .ForMember(id => id.NormalizedEmail, expr => expr.Ignore())
            .ForMember(id => id.EmailConfirmed, expr => expr.Ignore())
            .ForMember(id => id.PasswordHash, expr => expr.Ignore())
            .ForMember(id => id.SecurityStamp, expr => expr.Ignore())
            .ForMember(id => id.ConcurrencyStamp, expr => expr.Ignore())
            .ForMember(id => id.PhoneNumber, expr => expr.Ignore())
            .ForMember(id => id.PhoneNumberConfirmed, expr => expr.Ignore())
            .ForMember(id => id.TwoFactorEnabled, expr => expr.Ignore())
            .ForMember(id => id.LockoutEnd, expr => expr.Ignore())
            .ForMember(id => id.LockoutEnabled, expr => expr.Ignore())
            .ForMember(id => id.AccessFailedCount, expr => expr.Ignore())
            .ForMember(id => id.CreatedAt, expr => expr.Ignore())
            .ForMember(id => id.UpdatedAt, expr => expr.Ignore())
            .ForMember(id => id.RefreshTokens, expr => expr.Ignore())
            .ForMember(id => id.UserInfo, expr => expr.Ignore())
            ;
    }
}

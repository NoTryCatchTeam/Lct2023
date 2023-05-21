using AutoMapper;
using DataModel.Responses.Users;
using Lct2023.Definitions.Internal;

namespace Lct2023.Definitions;

public class AppMapperProfile : Profile
{
    public AppMapperProfile()
    {
        CreateMap<UserItemResponse, User>()
            .ForMember(x => x.AccessToken, expr => expr.Ignore())
            .ForMember(x => x.RefreshToken, expr => expr.Ignore())
            ;
    }
}

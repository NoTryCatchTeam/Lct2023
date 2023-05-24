using AutoMapper;
using DataModel.Responses.Map;
using DataModel.Responses.Users;
using Lct2023.Definitions.Enums;
using Lct2023.Definitions.Internals;
using Lct2023.ViewModels.Map;

namespace Lct2023.Definitions;

public class AppMapperProfile : Profile
{
    public AppMapperProfile()
    {
        CreateMap<UserItemResponse, User>()
            .ForMember(x => x.AccessToken, expr => expr.Ignore())
            .ForMember(x => x.RefreshToken, expr => expr.Ignore())
            ;
        
        
        CreateMap<SchoolLocationResponse, PlaceItemViewModel>()
            .ForMember(x => x.Title, expr => expr.MapFrom(x => x.Name))
            .ForMember(x => x.Description, expr => expr.MapFrom(_ => "Музыкальное образование, Дополнительное образование, Учебный центр, Управление образованием"))
            .ForMember(x => x.Address, expr => expr.MapFrom(_ => "ул. Воронцовская, д.30А"))
            .ForMember(x => x.Contacts, expr => expr.MapFrom(_ => new [] { "+7 (495) 912-14-28", "+7 (495) 912-14-28", "+7 (495) 912-14-28"}))
            .ForMember(x => x.Email, expr => expr.MapFrom(_ => "dmshmozarta@culture.mos.ru"))
            .ForMember(x => x.Site, expr => expr.MapFrom(_ => "http://www.balakirevschool.ru/"))
            .ForMember(x => x.SocialLinks, expr => expr.MapFrom(_ => new []
            {
                new SocialLinkItemViewModel
                {
                    SocialLinkType = SocialLinkTypes.Vk,
                    Url = "https://vk.com/"
                },
                new SocialLinkItemViewModel
                {
                    SocialLinkType = SocialLinkTypes.Youtube,
                    Url = "https://www.youtube.com/"
                },
            }))
            .ForMember(x => x.Events, expr => expr.Ignore())
            .ForMember(x => x.ArtDirections, expr => expr.MapFrom(_ => new [] { ArtDirectionType.Cello, ArtDirectionType.Drums, ArtDirectionType.Horn }))
            .ForMember(x => x.LocationType, expr => expr.MapFrom(_ => LocationType.School))
            ;
    }
}

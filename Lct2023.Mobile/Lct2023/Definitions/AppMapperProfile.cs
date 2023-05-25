using System;
using System.Linq;
using AutoMapper;
using DataModel.Definitions.Enums;
using DataModel.Responses.BaseCms;
using DataModel.Responses.Map;
using DataModel.Responses.Users;
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

        CreateMap<CmsItemResponse<SchoolLocationResponse>, PlaceItemViewModel>()
            .ValidateMemberList(MemberList.None)
            .AfterMap((s, d, c) => c.Mapper.Map(s.Item, d));
        
        CreateMap<SchoolLocationResponse, PlaceItemViewModel>()
            .ForMember(x => x.Id, expr => expr.Ignore())
            .ForMember(x => x.Title, expr => expr.MapFrom(x => x.Name))
            .ForMember(x => x.Description, expr => expr.MapFrom(_ => "Музыкальное образование, Дополнительное образование, Учебный центр, Управление образованием"))
            .ForMember(x => x.Contacts, expr => expr.MapFrom((s, d, _) => s.Phone?.Split(";", StringSplitOptions.RemoveEmptyEntries).ToArray()))
            .ForMember(x => x.Site, expr => expr.MapFrom(_ => "http://www.balakirevschool.ru/"))
            .ForMember(x => x.SocialLinks, expr => expr.MapFrom(_ => new []
            {
                new SocialLinkResponse
                {
                    SocialLinkType = SocialLinkTypes.Vk,
                    Url = "https://vk.com/"
                },
                new SocialLinkResponse
                {
                    SocialLinkType = SocialLinkTypes.Youtube,
                    Url = "https://www.youtube.com/"
                },
            }))
            .ForMember(x => x.Events, expr => expr.MapFrom(x => new []
            {
                new EventItemResponse
                {
                    Description = "4 июня, 13:00",
                    Title = "Cinema Orchestra Medl",
                    ImageUrl = "https://media.newyorker.com/photos/59095bb86552fa0be682d9d0/master/w_2560%2Cc_limit/Monkey-Selfie.jpg",
                    Url = "https://www.kassir.ru/"
                },
                new EventItemResponse
                {
                    Description = "4 июня, 13:00",
                    Title = "Cinema Orchestra Medl",
                    ImageUrl = "https://media.newyorker.com/photos/59095bb86552fa0be682d9d0/master/w_2560%2Cc_limit/Monkey-Selfie.jpg",
                    Url = "https://www.kassir.ru/"
                },
            }))
            .ForMember(x => x.ArtDirections, expr => expr.MapFrom(_ => new [] { ArtDirectionType.Cello, ArtDirectionType.Drums, ArtDirectionType.Horn }))
            .ForMember(x => x.LocationType, expr => expr.MapFrom(_ => LocationType.School))
            ;

        CreateMap<SocialLinkResponse, SocialLinkItemViewModel>()
            .ValidateMemberList(MemberList.Source);
        
        CreateMap<EventItemResponse, EventItemViewModel>()
            .ValidateMemberList(MemberList.Source);
    }
}

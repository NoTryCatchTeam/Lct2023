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
    private const string DEFAULT_PIN_HEX_COLOR = "#8fb14c";
    
    public AppMapperProfile()
    {
        CreateMap<UserItemResponse, User>()
            .ForMember(x => x.AccessToken, expr => expr.Ignore())
            .ForMember(x => x.RefreshToken, expr => expr.Ignore())
            ;

        CreateMap<CmsItemResponse<SchoolLocationResponse>, MapSearchResultItemViewModel>()
            .ValidateMemberList(MemberList.None)
            .AfterMap((s, d, c) => c.Mapper.Map(s.Item, d));

        CreateMap<CmsItemResponse<EventItemResponse>, MapSearchResultItemViewModel>()
            .ValidateMemberList(MemberList.None)
            .AfterMap((s, d, c) => c.Mapper.Map(s.Item, d));

        CreateMap<CmsItemResponse<SchoolLocationResponse>, PlaceItemViewModel>()
            .ValidateMemberList(MemberList.None)
            .AfterMap((s, d, c) => c.Mapper.Map(s.Item, d));
        

        CreateMap<CmsItemResponse<EventItemResponse>, PlaceItemViewModel>()
            .ValidateMemberList(MemberList.None)
            .AfterMap((s, d, c) => c.Mapper.Map(s.Item, d));

        CreateMap<SchoolLocationResponse, MapSearchResultItemViewModel>()
            .ForMember(x => x.Id, expr => expr.Ignore())
            .ForMember(x => x.HexColor, expr => expr.MapFrom(s => GetHexColorForSchool(s)))
            .ForMember(x => x.Title, expr => expr.MapFrom(x => x.Name))
            .ForMember(x => x.LocationType, expr => expr.MapFrom(_ => LocationType.School))
            ;
        
        CreateMap<EventItemResponse, MapSearchResultItemViewModel>()
            .ForMember(x => x.Id, expr => expr.Ignore())
            .ForMember(x => x.HexColor, expr => expr.MapFrom(s => DEFAULT_PIN_HEX_COLOR))
            .ForMember(x => x.Title, expr => expr.MapFrom(x => x.Name))
            .ForMember(x => x.Address, expr => expr.MapFrom((s, d, _) => s?.Place?.Data?.Item?.Address))
            .ForMember(x => x.LocationType, expr => expr.MapFrom(_ => LocationType.Event))
            ;
        
        CreateMap<SchoolLocationResponse, PlaceItemViewModel>()
            .ForMember(x => x.Id, expr => expr.Ignore())
            .ForMember(x => x.HexColor, expr => expr.MapFrom(s => GetHexColorForSchool(s)))
            .ForMember(x => x.TicketLink, expr => expr.Ignore())
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
            .ForMember(x => x.ArtDirections, expr => expr.MapFrom(_ => new [] { ArtDirectionType.Cello, ArtDirectionType.Drums, ArtDirectionType.Horn }))
            .ForMember(x => x.LocationType, expr => expr.MapFrom(_ => LocationType.School))
            ;
        
        CreateMap<EventItemResponse, PlaceItemViewModel>()
            .ForMember(x => x.HexColor, expr => expr.MapFrom(s => DEFAULT_PIN_HEX_COLOR))
            .ForMember(x => x.Address, expr => expr.MapFrom((s, d, _) => s?.Place?.Data?.Item?.Address))
            .ForMember(x => x.Longitude, expr => expr.MapFrom((s, d, _) => s?.Place?.Data?.Item?.Longitude))
            .ForMember(x => x.Latitude, expr => expr.MapFrom((s, d, _) => s?.Place?.Data?.Item?.Latitude))
            .ForMember(x => x.Id, expr => expr.Ignore())
            .ForMember(x => x.Contacts, expr => expr.MapFrom((s, d, _) => s.Phone?.Split(";", StringSplitOptions.RemoveEmptyEntries).ToArray()))
            .ForMember(x => x.Email, expr => expr.Ignore())
            .ForMember(x => x.Title, expr => expr.MapFrom(x => x.Name))
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
            .ForMember(x => x.ArtDirections, expr => expr.Ignore())
            .ForMember(x => x.LocationType, expr => expr.MapFrom(_ => LocationType.Event))
            ;

        CreateMap<SocialLinkResponse, SocialLinkItemViewModel>()
            .ValidateMemberList(MemberList.Source);
    }

    private string GetHexColorForSchool(SchoolLocationResponse school)
    {
        return school?.District?.Data?.Item?.DistrictType switch
        {
            _ when school?.IsSpecial == true => DEFAULT_PIN_HEX_COLOR,
            DistrictType.Cao => DEFAULT_PIN_HEX_COLOR,
            DistrictType.Sao => "#108baf",
            DistrictType.Svao => "#8b72ad",
            DistrictType.Vao => "#b378b5",
            DistrictType.Uvao => "#c18b9d",
            DistrictType.Uao => "#f9a98f",
            DistrictType.Uzao => "#6eb17e",
            DistrictType.Zao => "#4e3660",
            DistrictType.Szao => "#932689",
            DistrictType.Zelao => "#6ba751",
            DistrictType.Tinao => "#dfa5bb",
            null => DEFAULT_PIN_HEX_COLOR,
            _ => DEFAULT_PIN_HEX_COLOR
        };
    }
}

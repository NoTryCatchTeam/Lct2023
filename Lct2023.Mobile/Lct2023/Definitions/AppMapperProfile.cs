using System;
using System.Linq;
using AutoMapper;
using DataModel.Definitions.Enums;
using DataModel.Responses.Art;
using DataModel.Responses.BaseCms;
using DataModel.Responses.Feed;
using DataModel.Responses.Map;
using DataModel.Responses.Users;
using Lct2023.Commons.Extensions;
using Lct2023.Definitions.Constants;
using Lct2023.Definitions.Internals;
using Lct2023.ViewModels.Art;
using Lct2023.ViewModels.Common;
using Lct2023.ViewModels.Feed;
using Lct2023.ViewModels.Map;
using Microsoft.Extensions.Configuration;
using MvvmCross;

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

        CreateMap<CmsItemResponse<StreamResponse>, StreamItemViewModel>()
            .ForMember(x => x.Name, expr => expr.MapFrom(s => s.Item.Name))
            ;

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
            .ForMember(x => x.LocationType, expr => expr.MapFrom((s) => LocationType.School))
            .ForMember(x => x.Streams, expr => expr.MapFrom((s, d, _) => s.Streams?.Data))
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
            .ForMember(x => x.Streams, expr => expr.Ignore())
            .ForMember(x => x.LocationType, expr => expr.MapFrom(_ => LocationType.Event))
            ;

        CreateMap<SocialLinkResponse, SocialLinkItemViewModel>()
            .ValidateMemberList(MemberList.Source);

        CreateMap<DistrictResponse, FilterItemViewModel>()
            .ForMember(x => x.Title, expr => expr.MapFrom(s => s.District))
            .ForMember(x => x.IsSelected, expr => expr.Ignore())
            ;

        CreateMap<StreamResponse, FilterItemViewModel>()
            .ForMember(x => x.Title, expr => expr.MapFrom(s => s.Name))
            .ForMember(x => x.IsSelected, expr => expr.Ignore())
            ;

        CreateMap<ArtCategoryResponse, FilterItemViewModel>()
            .ForMember(x => x.Title, expr => expr.MapFrom(s => s.DisplayName))
            .ForMember(x => x.IsSelected, expr => expr.Ignore())
            ;

        CreateMap<RubricResponse, FilterItemViewModel>()
            .ForMember(x => x.Title, expr => expr.MapFrom(s => s.Name))
            .ForMember(x => x.IsSelected, expr => expr.Ignore())
            ;
        
        CreateMap<CmsItemResponse<ArticleResponse>, FeedItemViewModel>()
            .ValidateMemberList(MemberList.None)
            .AfterMap((s, d, c) => c.Mapper.Map(s.Item, d));


        CreateMap<ArticleResponse, FeedItemViewModel>()
            .ForMember(x => x.Id, expr => expr.Ignore())
            .ForMember(x => x.ImageUrl, expr => expr.MapFrom((s, d, _) => s.Cover?.Data?.Item?.Url?.Then(url =>
            {
                var configuration = Mvx.IoCProvider.Resolve<IConfiguration>();
                return $"{configuration.GetValue<string>(ConfigurationConstants.AppSettings.HOST)}{configuration.GetValue<string>(ConfigurationConstants.AppSettings.CMS_PATH)}{url}";
            })))
            .ForMember(x => x.ClickCommand, expr => expr.Ignore())
            ;
    }

    private string GetHexColorForSchool(SchoolLocationResponse school)
    {
        return school?.District?.Data?.Item?.AreaType switch
        {
            _ when school?.IsSpecial == true => DEFAULT_PIN_HEX_COLOR,
            AreaType.Cao => DEFAULT_PIN_HEX_COLOR,
            AreaType.Sao => "#108baf",
            AreaType.Svao => "#8b72ad",
            AreaType.Vao => "#b378b5",
            AreaType.Uvao => "#c18b9d",
            AreaType.Uao => "#f9a98f",
            AreaType.Uzao => "#6eb17e",
            AreaType.Zao => "#4e3660",
            AreaType.Szao => "#932689",
            AreaType.Zelao => "#6ba751",
            AreaType.Tinao => "#dfa5bb",
            null => DEFAULT_PIN_HEX_COLOR,
            _ => DEFAULT_PIN_HEX_COLOR
        };
    }
}

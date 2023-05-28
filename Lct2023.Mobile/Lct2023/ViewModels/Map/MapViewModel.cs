using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using Lct2023.Commons.Extensions;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DataModel.Definitions.Enums;
using DataModel.Responses.Art;
using DataModel.Responses.BaseCms;
using DataModel.Responses.Map;
using DynamicData;
using DynamicData.Binding;
using Lct2023.Business.RestServices.Art;
using Lct2023.Business.RestServices.Map;
using Lct2023.Business.Definitions;
using Lct2023.Definitions.Enums;
using Lct2023.Services;
using Lct2023.ViewModels.Common;
using Lct2023.ViewModels.Map.Filters;
using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using ReactiveUI;
using Xamarin.Essentials;
using Location = Xamarin.Essentials.Location;

namespace Lct2023.ViewModels.Map;

public class MapViewModel : BaseViewModel
{
    private const int PAGE_SIZE = 100;
    
    private readonly IMapRestService _mapRestService;
    private CmsItemResponse<SchoolLocationResponse>[] _schoolLocations;
    private CmsItemResponse<EventItemResponse>[] _events;
    private readonly IMapper _mapper;
    private IEnumerable<ContactItemViewModel> _contacts;
    private readonly IDialogService _dialogService;
    private readonly IArtRestService _artRestService;

    public MapViewModel(
        IMapRestService mapRestService,
        IArtRestService artRestService,
        ILoggerFactory logFactory,
        IMvxNavigationService navigationService,
        IDialogService dialogService,
        IXamarinEssentialsWrapper xamarinEssentialsWrapper,
        IMapper mapper)
        : base(logFactory, navigationService)
    {
        _artRestService = artRestService;
        _mapRestService = mapRestService;
        _mapper = mapper;
        _dialogService = dialogService;

        GpsCommand = new MvxAsyncCommand(() =>
            RunSafeTaskAsync(async () =>
            {
                await xamarinEssentialsWrapper.RunOnUiAsync(async () =>
                    IsMyLocationEnabled = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>() == PermissionStatus.Granted ||
                                          await Permissions.RequestAsync<Permissions.LocationWhenInUse>() == PermissionStatus.Granted);

                if (!IsMyLocationEnabled)
                {
                    dialogService.ShowToast("Для работы с картой необходимо предоставить доступ к геолокации");
                    AppInfo.ShowSettingsUI();
                    return;
                }
                
                dialogService.ShowToast("Определяем локацию");
                
                MyPosition = await xamarinEssentialsWrapper.GetCurrentLocationAsync(CancellationToken);
            }));
        
        CopyCommand = new MvxAsyncCommand<object>(async param =>
        {
            var text = param switch
            {
                LocationActionItem.Email => SelectedLocation?.Email,
                LocationActionItem.Address => SelectedLocation?.Address,
                LocationActionItem.Site => SelectedLocation?.Site,
                string @string => @string
            };
            await Clipboard.SetTextAsync(text);
            dialogService.ShowToast("Скопировано");
        });
        
        ExpandContactsCommand = new MvxAsyncCommand(async () =>
        {
            if (SelectedLocation?.Contacts?.Any() != true)
            {
                return;
            }
            
            UpdateContacts(Contacts?.Count() == 1
                ? SelectedLocation.Contacts.Count()
                : 1);
        });
        
        OpenMapCommand = new MvxAsyncCommand(() =>
        {
            if (SelectedLocation == null)
            {
                return Task.CompletedTask;
            }

            return RunSafeTaskAsync(() => Xamarin.Essentials.Map.OpenAsync(
                new Location(SelectedLocation.Latitude, SelectedLocation.Longitude),
                new MapLaunchOptions { Name = SelectedLocation.Address }));
        });
        
        OpenSiteCommand = new MvxAsyncCommand<object>( param =>
            {
                var site = param switch
                {
                    LocationActionItem => SelectedLocation?.Site,
                    string @string => @string
                };
                
                if (string.IsNullOrEmpty(site))
                {
                    return Task.CompletedTask;
                }
                
                return RunSafeTaskAsync(() => Browser.OpenAsync(site, BrowserLaunchMode.SystemPreferred));
            });
        
        CallCommand = new MvxAsyncCommand<string>( phone =>
        {
            if (string.IsNullOrEmpty(phone))
            {
                return Task.CompletedTask;
            }
            
            return RunSafeTaskAsync(async () => PhoneDialer.Open(phone));
        });
        
        OpenEmailCommand = new MvxAsyncCommand<object>(param =>
        {
            var text = param switch
            {
                LocationActionItem.Email => SelectedLocation?.Email,
                string @string => @string
            };
            
            if (string.IsNullOrEmpty(text))
            {
                return Task.CompletedTask;
            }
            
            return RunSafeTaskAsync(() =>
            {
                var message = new EmailMessage
                {
                    To = new List<string>
                    {
                        text
                    },
                };
                return Email.ComposeAsync(message);
            });
        });
        
        ActionCommand = new MvxAsyncCommand(async () =>
        {
            switch (SelectedLocation?.LocationType)
            {
                case LocationType.Event:
                    OpenSiteCommand.Execute(SelectedLocation.TicketLink);
                    break;
            }
        });

        this.WhenAnyValue(vm => vm.SelectedLocation)
            .WhereNotNull()
            .Subscribe(_ => UpdateContacts(1));

        this.WhenAnyValue(vm => vm.LocationType, vm => vm.SelectedFilters)
            .Subscribe(_ => UpdatePlaces());

        this.WhenAnyValue(vm => vm.SearchText)
            .Throttle(TimeSpan.FromMilliseconds(350), RxApp.MainThreadScheduler)
            .Subscribe(_ => UpdateSearchResults());
    }

    public ObservableCollection<PlaceItemViewModel> Places { get; } = new ();
    
    public ObservableCollection<MapFilterGroupItemViewModel> FilterGroups { get; } = new ();
    
    public ObservableCollection<(MapFilterGroupType FilterGroupType, string[] Items)> SelectedFilters { get; private set; }
    
    public IMvxCommand GpsCommand { get; }
    
    public IMvxCommand CopyCommand { get; }
    
    public IMvxCommand OpenMapCommand { get; }

    public IMvxCommand OpenSiteCommand { get; }
    
    public IMvxCommand OpenEmailCommand { get; }
    
    public IMvxCommand CallCommand { get; }

    public IMvxCommand ExpandContactsCommand { get; }

    public IMvxCommand ActionCommand { get; set; }
    
    public string SearchText { get; set; }

    public ObservableCollection<MapSearchResultItemViewModel> SearchResults { get; } = new();

    public string Image { get; private set; } =
        "https://media.newyorker.com/photos/59095bb86552fa0be682d9d0/master/w_2560%2Cc_limit/Monkey-Selfie.jpg";
    
    public IEnumerable<SocialLinkItemViewModel> SocialLinks => SelectedLocation?.SocialLinks?.Then(e
        => _mapper.Map<IEnumerable<SocialLinkItemViewModel>>(e, ops => ops.AfterMap((_, socialLinks) =>
        {
            foreach (var socialLink in socialLinks)
            {
                socialLink.OpenSiteCommand = OpenSiteCommand;
            }
        })));

    public IEnumerable<EventItemViewModel> Events => null;
        /*SelectedLocation?.Events?.Then(e
        => _mapper.Map<IEnumerable<EventItemViewModel>>(e, ops => ops.AfterMap((_, events) =>
        {
            foreach (var @event in events)
            {
                @event.OpenSiteCommand = OpenSiteCommand;
            }
        })));*/
    
    public PlaceItemViewModel SelectedLocation { get; set; }
    
    public Location MyPosition { get; private set; }
    
    public bool IsMyLocationEnabled { get; private set; }
    
    public LocationType LocationType { get; set; }
    
    public IEnumerable<ContactItemViewModel> Contacts
    {
        get => _contacts;
        set => SetProperty(ref _contacts, value);
    }

    public void ApplyFilters()
    {
        SelectedFilters = FilterGroups
            .Select(pFg => (
                pFg.FilterGroupType,
                Items: pFg.SubGroups?.SelectMany(pFgs =>
                    pFgs?.Items?.Where(pFgsi => pFgsi.IsSelected).Select(pFgsi => pFgsi.Title)).ToArray()))
            .Where(x => x.Items?.Any() == true)
            .ToObservableCollection();
    }
    
    public void ResetFilters()
    {
        FilterGroups.ForEach(fG =>
            fG?.SubGroups?.ForEach(fGs =>
                fGs?.Items?.ForEach(fGsi =>
                    fGsi.IsSelected = false)));

        SelectedFilters = null;
    }

    public override void ViewCreated()
    {
        base.ViewCreated();

        var schoolLocationTask = Task.Run(() => RunSafeTaskAsync(async () =>
        {
            _schoolLocations = (await _mapRestService.GetSchoolsLocationAsync(CancellationToken))?.ToArray();

            if (_schoolLocations?.Any() != true
                || LocationType == LocationType.Event)
            {
                return;
            }

            UpdatePlaces();
        }));
        
        var eventsTask = Task.Run(() => RunSafeTaskAsync(async () =>
        {
            _events = (await _mapRestService.GetEventsAsync(CancellationToken))?.ToArray();

            if (_events?.Any() != true
                || LocationType == LocationType.School)
            {
                return;
            }

            UpdatePlaces();
        }));
        
        var filtersTask = Task.Run(() => RunSafeTaskAsync(async () =>
        {
            IEnumerable<CmsItemResponse<DistrictResponse>> districts = null;
            IEnumerable<CmsItemResponse<StreamResponse>> streams = null;
            
            
            await Task.WhenAll(Task.Run(async () => districts = (await _mapRestService.LoadUntilEndAsync((rS, start) => rS.GetDistrictsPaginationAsync(start, PAGE_SIZE, CancellationToken))).ToArray()),
                Task.Run(async () => streams = (await _artRestService.GetStreamsAsync(CancellationToken))?.ToArray()));


            await InvokeOnMainThreadAsync(() =>
            {
                districts?.Where(d => d.Item?.AreaType != null).GroupBy(d => d.Item.AreaType, d => d.Item)
                    ?.Then(d => FilterGroups.Add(new MapFilterGroupItemViewModel
                    {
                        FilterGroupType = MapFilterGroupType.District,
                        Title = "Округа и районы",
                        SubGroups = d.Select(g => new MapFilterSubGroupItemViewModel
                        {
                            Title = g.Key.GetEnumMemberValue(),
                            Items = _mapper.Map<ObservableCollection<FilterItemViewModel>>(g)
                        }).ToObservableCollection(),
                    }));
                
                streams?.Where(d => d.Item?.ArtCategory?.Data?.Item?.DisplayName != null).GroupBy(d => d.Item.ArtCategory.Data.Item.DisplayName, d => d.Item)
                    ?.Then(d => FilterGroups.Add(new MapFilterGroupItemViewModel
                    {
                        FilterGroupType = MapFilterGroupType.Stream,
                        Title = "Направления",
                        SubGroups = d.Select(g => new MapFilterSubGroupItemViewModel
                        {
                            Title = g.Key,
                            Items = _mapper.Map<ObservableCollection<FilterItemViewModel>>(g)
                        }).ToObservableCollection(),
                    }));
            
                FilterGroups?.ForEach(fG =>
                    fG.SubGroups?.ForEach(fGs =>
                    {
                        var updateBusy = false;
                        fGs.WhenAnyValue(vm => vm.IsSelected)
                            .Where(_ => fGs.IsSelected != fGs.Items?.Any(fGsi => fGsi.IsSelected))
                            .Subscribe(_ =>
                            {
                                updateBusy = true;
                                fGs.Items?.ForEach(item => item.IsSelected = fGs.IsSelected);
                                updateBusy = false;
                            });
                        
                        fGs.Items
                            ?.ToObservableChangeSet()
                            .AutoRefresh(vm => vm.IsSelected)
                            .WhenAnyPropertyChanged()
                            .Select(_ => fGs.Items?.Any(fGsi => fGsi.IsSelected) == true)
                            .Where(isSelected => !updateBusy && fGs.IsSelected != isSelected)
                            .Subscribe(isSelected => fGs.IsSelected = isSelected);
                    }));
            });
        }));

        Task.WhenAny(schoolLocationTask, eventsTask, filtersTask);
    }

    private void UpdatePlaces()
    {
        Places.Clear();
        switch (LocationType)
        {
            case LocationType.Event:
                _events?.Then(es => Places.AddRange(_mapper.Map<IEnumerable<PlaceItemViewModel>>(es)));
                break;
            case LocationType.School when SelectedFilters?.Any() == true:
                _schoolLocations?.Where(sL =>
                    SelectedFilters.All(selectedFilter => selectedFilter.FilterGroupType switch
                    {
                        MapFilterGroupType.District => selectedFilter.Items.Contains(sL.Item.District.Data.Item.District),
                        MapFilterGroupType.Stream => selectedFilter.Items.Intersect(sL.Item.Streams.Data.Select(stream => stream.Item.Name)).Any(),
                    })).Then(sLs => Places.AddRange(_mapper.Map<IEnumerable<PlaceItemViewModel>>(sLs)));
                break;
            case LocationType.School:
                _schoolLocations?.Then(sLs => Places.AddRange(_mapper.Map<IEnumerable<PlaceItemViewModel>>(sLs)));
                break;
        }
    }
    
    private void UpdateSearchResults()
    {
        SearchResults.Clear();

        if (string.IsNullOrWhiteSpace(SearchText))
        {
            return;
        }

        switch (LocationType)
        {
            case LocationType.School:
                _schoolLocations?.Where(school => school?.Item?.Name?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true)
                    .Then(sLs => SearchResults.AddRange(_mapper.Map<IEnumerable<MapSearchResultItemViewModel>>(sLs)));
                break;
            case LocationType.Event:
                _events?.Where(e => e?.Item?.Name?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true
                    || e?.Item?.Description?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true)
                    .Then(es => SearchResults.AddRange(_mapper.Map<IEnumerable<MapSearchResultItemViewModel>>(es)));
                break;
        }
    }

    private void UpdateContacts(int count)
    {
        Contacts = SelectedLocation?.Contacts?.Take(count).Select(x => new ContactItemViewModel(x)
        {
            CopyCommand = new MvxAsyncCommand(async () =>
            {
                await Clipboard.SetTextAsync(x);
                _dialogService.ShowToast("Скопировано");
            }),
            CallCommand = new MvxAsyncCommand(() =>
            {
                if (string.IsNullOrEmpty(x))
                {
                    return Task.CompletedTask;
                }
            
                return RunSafeTaskAsync(async () => PhoneDialer.Open(x));
            }),
        }).ToArray();
    }

    public enum LocationActionItem
    {
        Address,
        
        Email,
        
        Site
    }
}

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using AutoMapper;
using DataModel.Definitions.Enums;
using DataModel.Responses.BaseCms;
using DataModel.Responses.Map;
using DynamicData;
using DynamicData.Binding;
using Lct2023.Business.RestServices.Map;
using Lct2023.Commons.Extensions;
using Lct2023.Services;
using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using ReactiveUI;
using Xamarin.Essentials;
using Location = Xamarin.Essentials.Location;

namespace Lct2023.ViewModels.Map;

public class MapViewModel : BaseViewModel
{
    private readonly IMapRestService _mapRestService;
    private CmsItemResponse<SchoolLocationResponse>[] _schoolLocations;
    private CmsItemResponse<EventItemResponse>[] _events;
    private readonly IMapper _mapper;
    private IEnumerable<ContactItemViewModel> _contacts;
    private readonly IDialogService _dialogService;

    public MapViewModel(
        IMapRestService mapRestService,
        ILoggerFactory logFactory,
        IMvxNavigationService navigationService,
        IDialogService dialogService,
        IXamarinEssentialsWrapper xamarinEssentialsWrapper,
        IMapper mapper)
        : base(logFactory, navigationService)
    {
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

        this.WhenValueChanged(vm => vm.SelectedLocation)
            .WhereNotNull()
            .Subscribe(_ => UpdateContacts(1));

        this.WhenValueChanged(vm => vm.LocationType)
            .Subscribe(_ => UpdatePlaces());
    }

    public ObservableCollection<PlaceItemViewModel> Places { get; } = new ();
    
    public IMvxCommand GpsCommand { get; }
    
    public IMvxCommand CopyCommand { get; }
    
    public IMvxCommand OpenMapCommand { get; }

    public IMvxCommand OpenSiteCommand { get; }
    
    public IMvxCommand OpenEmailCommand { get; }
    
    public IMvxCommand CallCommand { get; }

    public IMvxCommand ExpandContactsCommand { get; }

    public IMvxCommand ActionCommand { get; set; }
    
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

        Task.WhenAny(schoolLocationTask, eventsTask);
    }

    private void UpdatePlaces()
    {
        Places.Clear();
        switch (LocationType)
        {
            case LocationType.School:
                _schoolLocations?.Then(sLs => Places.AddRange(_mapper.Map<IEnumerable<PlaceItemViewModel>>(sLs)));
                break;
            case LocationType.Event:
                _events?.Then(es => Places.AddRange(_mapper.Map<IEnumerable<PlaceItemViewModel>>(es)));
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

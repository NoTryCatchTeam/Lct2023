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
using static Lct2023.ViewModels.Map.MapViewModel;
using Lct2023.Definitions.VmLinks;

namespace Lct2023.ViewModels.Map
{
    public class LocationsMapViewModel : BaseViewModel<LocationsMapVmLink>
    {
        private readonly IMapper _mapper;
        private readonly IDialogService _dialogService;

        private Func<Task<IEnumerable<PlaceItemViewModel>>> _placesFactory;
        private IEnumerable<ContactItemViewModel> _contacts;

        public LocationsMapViewModel(
            ILoggerFactory logFactory,
            IMvxNavigationService navigationService,
            IDialogService dialogService,
            IXamarinEssentialsWrapper xamarinEssentialsWrapper,
            IMapper mapper)
            : base(logFactory, navigationService)
        {
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

            OpenSiteCommand = new MvxAsyncCommand<object>(param =>
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

            CallCommand = new MvxAsyncCommand<string>(phone =>
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
        }

        public ObservableCollection<PlaceItemViewModel> Places { get; } = new();

        public IMvxCommand GpsCommand { get; }

        public IMvxCommand CopyCommand { get; }

        public IMvxCommand OpenMapCommand { get; }

        public IMvxCommand OpenSiteCommand { get; }

        public IMvxCommand OpenEmailCommand { get; }

        public IMvxCommand CallCommand { get; }

        public IMvxCommand ExpandContactsCommand { get; }

        public IMvxCommand ActionCommand { get; set; }

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

        public override void Prepare(LocationsMapVmLink parameter)
        {
            base.Prepare(parameter);
            LocationType = parameter.LocationType;
            _placesFactory = parameter.PlacesFactory;
        }

        public override Task Initialize()
        {
            return Task.WhenAny(RunSafeTaskAsync(InitializeItemsAsync),
                base.Initialize());
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

        private async Task InitializeItemsAsync()
        {
            var places = await _placesFactory();
            if (places?.Any() != true)
            {
                return;
            }


            await InvokeOnMainThreadAsync(() => Places.AddRange(places));
        }
    }
}


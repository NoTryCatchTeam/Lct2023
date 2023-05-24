using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using DataModel.Responses.BaseCms;
using DataModel.Responses.Map;
using DynamicData;
using Lct2023.Business.RestServices.Map;
using Lct2023.Services;
using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using Xamarin.Essentials;

namespace Lct2023.ViewModels.Map;

public class MapViewModel : BaseViewModel
{
    private readonly IMapRestService _mapRestService;
    private CmsItemResponse<SchoolLocationResponse>[] _schoolLocations;
    private readonly IMapper _mapper;

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

        GpsCommand = new MvxAsyncCommand(() =>
            RunSafeTaskAsync(async () =>
            {
                await xamarinEssentialsWrapper.RunOnUiAsync(async () =>
                    IsMyLocationEnabled = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>() == PermissionStatus.Granted ||
                                          await Permissions.RequestAsync<Permissions.LocationWhenInUse>() == PermissionStatus.Granted);
                
                dialogService.ShowToast("Определяем локацию");
                
                MyPosition = await xamarinEssentialsWrapper.GetCurrentLocationAsync(CancellationToken);
            }));
        
        FiltersCommand = new MvxAsyncCommand(async () =>
        {
            
        });
        
        CopyCommand = new MvxAsyncCommand<string>(async text =>
        {
            await Clipboard.SetTextAsync(text);
            dialogService.ShowToast("Скопировано");
        });
    }

    public ObservableCollection<PlaceItemViewModel> Places { get; } = new ();
    
    public IMvxCommand GpsCommand { get; }
    
    public IMvxCommand FiltersCommand { get; }
    
    public IMvxCommand CopyCommand { get; }
    
    public IMvxCommand OpenMapCommand { get; }
    
    public PlaceItemViewModel SelectedLocation { get; set; }
    
    public Location MyPosition { get; private set; }
    
    public bool IsMyLocationEnabled { get; private set; }

    public override void ViewCreated()
    {
        base.ViewCreated();
        
        Task.Run(() => RunSafeTaskAsync(async () =>
        {
            _schoolLocations = (await _mapRestService.GetSchoolsLocationAsync(CancellationToken))?.ToArray();

            if (_schoolLocations?.Any() != true)
            {
                return;
            }

            Places.AddRange(_mapper.Map<IEnumerable<PlaceItemViewModel>>(_schoolLocations));
        }));
    }
}

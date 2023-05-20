using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DataModel.Responses.BaseCms;
using DataModel.Responses.Map;
using DynamicData;
using Lct2023.Business.RestServices.Map;
using Lct2023.Services;
using Microsoft.Extensions.Logging;
using MvvmCross.Navigation;

namespace Lct2023.ViewModels.Map;

public class MapViewModel : BaseViewModel
{
    private readonly IXamarinEssentialsWrapper _xamarinEssentialsWrapper;
    private readonly IMapRestService _mapRestService;
    private CmsItemResponse<SchoolLocationResponse>[] _schoolLocations;

    public MapViewModel(
        IMapRestService mapRestService,
        ILoggerFactory logFactory,
        IMvxNavigationService navigationService,
        IXamarinEssentialsWrapper xamarinEssentialsWrapper)
        : base(logFactory, navigationService)
    {
        _xamarinEssentialsWrapper = xamarinEssentialsWrapper;
        _mapRestService = mapRestService;
    }

    public ObservableCollection<PlaceItemViewModel> Places { get; set; } = new();

    public override void ViewCreated()
    {
        base.ViewCreated();
        Task.Run(() => RunSafeTaskAsync(async () =>
        {
            _schoolLocations = (await _mapRestService.GetSchoolsLocationAsync())?.ToArray();

            if (_schoolLocations?.Any() != true)
            {
                return;
            }
            
            _xamarinEssentialsWrapper.RunOnUi(() => Places.AddRange(_schoolLocations.Select(schoolLocation => new PlaceItemViewModel(schoolLocation.Item))));
        }));
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lct2023.Business.RestServices.Map;
using Lct2023.Services;
using Microsoft.Extensions.Logging;
using MvvmCross.Navigation;
using PropertyChanged;

namespace Lct2023.ViewModels.Map;

public class MapViewModel : BaseViewModel
{
    private readonly IXamarinEssentialsWrapper _xamarinEssentialsWrapper;
    private readonly IMapRestService _mapRestService;

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
    
    public IReadOnlyCollection<SchoolLocationItemViewModel> Schools { get; set; }

    public override void ViewCreated()
    {
        base.ViewCreated();
        Task.Run(() => RunSafeTaskAsync(async () =>
        {
            var schoolLocations = (await _mapRestService.GetSchoolsLocationAsync())?.ToArray();

            if (schoolLocations?.Any() != true)
            {
                return;
            }
            
            _xamarinEssentialsWrapper.RunOnUi(() => 
                Schools = schoolLocations?.Select(schoolLocation => new SchoolLocationItemViewModel(schoolLocation.Item)).ToArray());
        }));
    }
}
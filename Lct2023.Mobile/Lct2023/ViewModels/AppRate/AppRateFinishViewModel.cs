using System.Threading;
using System.Threading.Tasks;
using Lct2023.Services;
using Microsoft.Extensions.Logging;
using MvvmCross.Navigation;

namespace Lct2023.ViewModels.AppRate;

public class AppRateFinishViewModel : BaseViewModel
{
    private readonly IUserService _userService;

    public AppRateFinishViewModel(
        IUserService userService,
        ILoggerFactory logFactory,
        IMvxNavigationService navigationService)
        : base(logFactory, navigationService)
    {
        _userService = userService;
    }

    public override void ViewCreated()
    {
        base.ViewCreated();

        RunSafeTaskAsync(() => _userService.UpdateRatingAsync(100, CancellationToken.None));
    }
}

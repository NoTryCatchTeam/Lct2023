using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.Navigation;

namespace Lct2023.ViewModels.AppRate;

public class AppRateViewModel : BaseViewModel
{
    public AppRateViewModel(ILoggerFactory logFactory, IMvxNavigationService navigationService)
        : base(logFactory, navigationService)
    {
        SendCommand = new MvxAsyncCommand(
            async () =>
            {
                await NavigationService.Close(this);
                await NavigationService.Navigate<AppRateFinishViewModel>();
            });
    }

    public IMvxAsyncCommand SendCommand { get; }
}

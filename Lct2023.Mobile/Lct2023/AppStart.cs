using System.Threading.Tasks;
using Lct2023.ViewModels;
using Lct2023.ViewModels.Map;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace Lct2023;

public class AppStart : MvxAppStart
{
    public AppStart(IMvxApplication application, IMvxNavigationService navigationService)
        : base(application, navigationService)
    {
    }

    protected override Task NavigateToFirstViewModel(object hint = null)
    {
        return NavigationService.Navigate<MapViewModel>();
    }
}

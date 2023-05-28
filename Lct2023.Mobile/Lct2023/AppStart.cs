using System.Threading.Tasks;
using Lct2023.Services;
using Lct2023.ViewModels;
using Lct2023.ViewModels.Auth;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Xamarin.Essentials;

namespace Lct2023;

public class AppStart : MvxAppStart
{
    private readonly IUserContext _userContext;

    public AppStart(IUserContext userContext, IMvxApplication application, IMvxNavigationService navigationService)
        : base(application, navigationService)
    {
        _userContext = userContext;
    }

    protected override async Task NavigateToFirstViewModel(object hint = null)
    {
        await _userContext.RestoreAsync();

        if (_userContext.IsAuthenticated)
        {
            await NavigationService.Navigate<MainTabbedViewModel>();
        }
        else
        {
            await NavigationService.Navigate<AuthViewModel>();
        }
    }
}

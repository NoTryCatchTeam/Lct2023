using System;
using System.Threading;
using System.Threading.Tasks;
using Lct2023.Business.RestServices.Users;
using Lct2023.Helpers;
using Lct2023.Services;
using Lct2023.ViewModels;
using Lct2023.ViewModels.Auth;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Xamarin.Essentials;

namespace Lct2023;

public class AppStart : MvxAppStart
{
    private readonly IUserService _userService;
    private readonly IUserContext _userContext;

    public AppStart(
        IUserService userService,
        IUserContext userContext,
        IMvxApplication application,
        IMvxNavigationService navigationService)
        : base(application, navigationService)
    {
        _userService = userService;
        _userContext = userContext;
    }

    protected override async Task NavigateToFirstViewModel(object hint = null)
    {
        await _userContext.RestoreAsync();

        if (_userContext.IsAuthenticated)
        {
            // Try to update user's rating
            Task.Run(
                async () =>
                {
                    try
                    {
                        await _userService.UpdateRatingAsync(CancellationToken.None)
                            .TimeoutAfter(TimeSpan.FromSeconds(10));
                    }
                    catch (Exception ex)
                    {
                        // ignored
                    }
                });

            await NavigationService.Navigate<MainTabbedViewModel>();
        }
        else
        {
            await NavigationService.Navigate<AuthViewModel>();
        }
    }
}

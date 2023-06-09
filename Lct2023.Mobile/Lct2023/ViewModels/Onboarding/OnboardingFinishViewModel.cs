using System.Threading;
using Lct2023.Services;
using Microsoft.Extensions.Logging;
using MvvmCross.Navigation;

namespace Lct2023.ViewModels.Onboarding;

public class OnboardingFinishViewModel : BaseViewModel
{
    private readonly IUserService _userService;

    public OnboardingFinishViewModel(
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

        RunSafeTaskAsync(() => _userService.UpdateRatingAsync(150, CancellationToken.None));
    }
}

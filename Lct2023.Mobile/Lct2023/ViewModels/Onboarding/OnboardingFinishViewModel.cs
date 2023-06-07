using Microsoft.Extensions.Logging;
using MvvmCross.Navigation;

namespace Lct2023.ViewModels.Onboarding;

public class OnboardingFinishViewModel : BaseViewModel
{
    public OnboardingFinishViewModel(ILoggerFactory logFactory, IMvxNavigationService navigationService)
        : base(logFactory, navigationService)
    {
    }
}

using System.Threading.Tasks;
using Lct2023.ViewModels.Onboarding;
using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.Navigation;

namespace Lct2023.ViewModels;

public class MainTabbedViewModel : BaseViewModel
{
    private IMvxAsyncCommand _finishOnboardingCommand;

    public MainTabbedViewModel(ILoggerFactory logFactory, IMvxNavigationService navigationService)
        : base(logFactory, navigationService)
    {
        FinishOnboardingCommand = new MvxAsyncCommand(FinishOnboardingAsync);
    }

    private Task FinishOnboardingAsync() =>
        NavigationService.Navigate<OnboardingFinishViewModel>();

    public IMvxAsyncCommand FinishOnboardingCommand { get; }
}

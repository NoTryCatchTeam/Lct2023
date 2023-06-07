using System.Threading.Tasks;
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
        SkipOnboardingCommand = new MvxAsyncCommand(SkipOnboardingAsync);
        FinishOnboardingCommand = new MvxAsyncCommand(FinishOnboardingAsync);
    }

    private Task FinishOnboardingAsync()
    {
        return NavigationService.Navigate<>();
    }

    private Task SkipOnboardingAsync()
    {
        return Task.CompletedTask;
    }

    public IMvxAsyncCommand SkipOnboardingCommand { get; }

    public IMvxAsyncCommand FinishOnboardingCommand { get; }
}

using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.Navigation;

namespace Lct2023.ViewModels.ProfTest;

public class ProfTestStep2ViewModel : BaseViewModel
{
    public ProfTestStep2ViewModel(ILoggerFactory logFactory, IMvxNavigationService navigationService)
        : base(logFactory, navigationService)
    {
        NextCommand = new MvxAsyncCommand(
            async () =>
            {
                await NavigationService.Close(this);
                await NavigationService.Navigate<ProfTestStep3ViewModel>();
            });
    }

    public IMvxAsyncCommand NextCommand { get; }
}

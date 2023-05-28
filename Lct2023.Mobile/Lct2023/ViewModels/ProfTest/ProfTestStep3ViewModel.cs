using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.Navigation;

namespace Lct2023.ViewModels.ProfTest;

public class ProfTestStep3ViewModel : BaseViewModel
{
    public ProfTestStep3ViewModel(ILoggerFactory logFactory, IMvxNavigationService navigationService)
        : base(logFactory, navigationService)
    {
        NextCommand = new MvxAsyncCommand(
            async () =>
            {
                await NavigationService.Close(this);
                await NavigationService.Navigate<ProfTestFinishViewModel>();
            });
    }

    public IMvxAsyncCommand NextCommand { get; }
}

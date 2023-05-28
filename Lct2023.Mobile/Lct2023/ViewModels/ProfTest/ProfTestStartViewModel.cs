using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.Navigation;

namespace Lct2023.ViewModels.ProfTest;

public class ProfTestStartViewModel : BaseViewModel
{
    public ProfTestStartViewModel(ILoggerFactory logFactory, IMvxNavigationService navigationService)
        : base(logFactory, navigationService)
    {
        StartCommand = new MvxAsyncCommand(
            async () =>
            {
                await NavigationService.Close(this);
                await NavigationService.Navigate<ProfTestStep1ViewModel>();
            });
    }

    public IMvxAsyncCommand StartCommand { get; }
}

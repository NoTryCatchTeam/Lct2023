using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.Navigation;

namespace Lct2023.ViewModels.ProfTest;

public class ProfTestStep1ViewModel : BaseViewModel
{
    public ProfTestStep1ViewModel(ILoggerFactory logFactory, IMvxNavigationService navigationService)
        : base(logFactory, navigationService)
    {
        NextCommand = new MvxAsyncCommand(
            async () =>
            {
                await NavigationService.Close(this);
                await NavigationService.Navigate<ProfTestStep2ViewModel>();
            });

        Options = new[]
        {
            "Музыка",
            "Хореография",
            "Живопись",
            "Театр",
            "Архитектура",
            "Дизайн",
        };
    }

    public IMvxAsyncCommand NextCommand { get; }
    
    public IEnumerable<string> Options { get; }
}

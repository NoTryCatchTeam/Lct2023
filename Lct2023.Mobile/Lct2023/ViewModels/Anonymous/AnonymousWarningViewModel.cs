using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.Navigation;

namespace Lct2023.ViewModels.Anonymous;

public class AnonymousWarningViewModel : BaseViewModelResult<AnonymousWarningViewModel.NavResult>
{
    public AnonymousWarningViewModel(ILoggerFactory logFactory, IMvxNavigationService navigationService)
        : base(logFactory, navigationService)
    {
        ContinueCommand = new MvxAsyncCommand(() => NavigationService.Close(this, new NavResult(true)));
    }

    public IMvxAsyncCommand ContinueCommand { get; }

    protected override Task NavigateBackAction()
    {
        return NavigationService.Close(this, new NavResult(false));
    }

    public class NavResult
    {
        public NavResult(bool isContinue)
        {
            IsContinue = isContinue;
        }

        public bool IsContinue { get; }
    }
}

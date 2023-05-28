using Microsoft.Extensions.Logging;
using MvvmCross.Navigation;

namespace Lct2023.ViewModels.Common;

public class WebViewModel : BaseViewModel<WebViewModel.NavParameter>
{
    public WebViewModel(ILoggerFactory logFactory, IMvxNavigationService navigationService)
        : base(logFactory, navigationService)
    {
    }

    public class NavParameter
    {
        public NavParameter(string url)
        {
            Url = url;
        }

        public string Url { get; }
    }

    public ILogger Logger => Log;
}

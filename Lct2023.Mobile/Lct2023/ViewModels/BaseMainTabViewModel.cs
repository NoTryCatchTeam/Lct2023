using System;
using Lct2023.Definitions.VmResult;
using Lct2023.ViewModels.Profile;
using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.Navigation;

namespace Lct2023.ViewModels
{
    public class BaseMainTabViewModel : BaseViewModel
    {
        public IMvxCommand GoToProfileCommand { get; }

        protected BaseMainTabViewModel(ILoggerFactory logFactory, IMvxNavigationService navigationService)
            : base(logFactory, navigationService)
        {

            GoToProfileCommand = new MvxAsyncCommand<Action<ProfileResult>>(async onProfileResultAction =>
            {
                if (await NavigationService.Navigate<ProfileViewModel, ProfileResult>() is not { } result)
                {
                    return;
                }

                onProfileResultAction?.Invoke(result);
            });
        }
    }
}


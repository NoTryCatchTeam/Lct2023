using System;
using System.Threading.Tasks;
using AutoMapper;
using Lct2023.Business.RestServices.Art;
using Lct2023.Business.RestServices.Feed;
using Microsoft.Extensions.Logging;
using MvvmCross.Navigation;

namespace Lct2023.ViewModels.Feed
{
	public class MainFeedViewModel : BaseViewModel
    {
        public MainFeedViewModel(
        ILoggerFactory logFactory,
        IMvxNavigationService navigationService)
        : base(logFactory, navigationService)
        {
		}

        public override async void ViewCreated()
        {
            base.ViewCreated();
            await InvokeOnMainThreadAsync(() => NavigationService.Navigate<FeedViewModel>());
        }
    }
}


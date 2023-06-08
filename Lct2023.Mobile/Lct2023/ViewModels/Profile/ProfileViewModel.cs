using System;
using AutoMapper;
using Lct2023.Business.RestServices.Art;
using Lct2023.Business.RestServices.Feed;
using Lct2023.Business.RestServices.Map;
using Lct2023.Commons.Extensions;
using Lct2023.Definitions.Enums;
using Lct2023.Definitions.VmLinks;
using Lct2023.ViewModels.Feed;
using Lct2023.ViewModels.Map;
using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace Lct2023.ViewModels.Profile
{
	public class ProfileViewModel : BaseViewModel
    {
        public IMvxAsyncCommand RatingTableCommand { get; }

        public IMvxAsyncCommand SettingsCommand { get; }

        public ProfileViewModel(
            ILoggerFactory logFactory,
            IMvxNavigationService navigationService)
            : base(logFactory, navigationService)
        {
            RatingTableCommand = new MvxAsyncCommand(async () =>
            {

            });

            SettingsCommand = new MvxAsyncCommand(async () =>
            {

            });
        }

        public override Task Initialize()
        {
            return Task.WhenAny(base.Initialize());
        }
    }
}


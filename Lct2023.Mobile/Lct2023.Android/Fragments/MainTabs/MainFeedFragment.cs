using System;
using Android.OS;
using Android.Views;
using Lct2023.Android.Fragments.Feed;
using Lct2023.Definitions.VmLinks;
using Lct2023.ViewModels.Feed;
using Lct2023.ViewModels.Main;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Platforms.Android.Presenters;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views;

namespace Lct2023.Android.Fragments.MainTabs
{
    [MvxFragmentPresentation]
    public class MainFeedFragment : BaseFragment<MainFeedViewModel>
    {
        protected override int GetLayoutId() => Resource.Layout.MainFeedFragment;
    }
}


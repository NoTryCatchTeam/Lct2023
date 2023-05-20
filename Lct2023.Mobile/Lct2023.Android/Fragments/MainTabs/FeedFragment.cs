using Lct2023.ViewModels.Feed;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace Lct2023.Android.Fragments.MainTabs;

[MvxFragmentPresentation]
public class FeedFragment : BaseFragment<FeedViewModel>
{
    protected override int GetLayoutId() => Resource.Layout.FeedFragment;
}

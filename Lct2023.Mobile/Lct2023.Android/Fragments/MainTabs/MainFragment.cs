using Lct2023.ViewModels.Main;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace Lct2023.Android.Fragments.MainTabs;

[MvxFragmentPresentation]
public class MainFragment : BaseFragment<MainViewModel>
{
    protected override int GetLayoutId() => Resource.Layout.MainFragment;
}

using Lct2023.ViewModels.Map;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace Lct2023.Android.Fragments.MainTabs;

[MvxFragmentPresentation]
public class MapFragment : BaseFragment<MapViewModel>
{
    protected override int GetLayoutId() => Resource.Layout.MapFragment;
}

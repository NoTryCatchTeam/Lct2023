using Lct2023.ViewModels.Courses;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace Lct2023.Android.Fragments.MainTabs;

[MvxFragmentPresentation]
public class CoursesFragment : BaseFragment<CoursesViewModel>
{
    protected override int GetLayoutId() => Resource.Layout.CoursesFragment;
}

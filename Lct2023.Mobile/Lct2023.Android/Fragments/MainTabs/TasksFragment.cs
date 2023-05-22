using Lct2023.ViewModels.Tests;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace Lct2023.Android.Fragments.MainTabs;

[MvxFragmentPresentation]
public class TasksFragment : BaseFragment<TasksViewModel>
{
    protected override int GetLayoutId() => Resource.Layout.TasksFragment;
}

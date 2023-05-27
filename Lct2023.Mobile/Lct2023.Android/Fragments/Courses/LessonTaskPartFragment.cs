using Android.OS;
using Android.Views;
using Android.Widget;
using Lct2023.ViewModels.Courses;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views.Fragments;

namespace Lct2023.Android.Fragments.Courses;

[MvxFragmentPresentation]
public class LessonTaskPartFragment : MvxFragment
{
    private CourseLessonItem _viewModel;

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        base.OnCreateView(inflater, container, savedInstanceState);

        _viewModel = (CourseLessonItem)DataContext;

        var view = this.BindingInflate(Resource.Layout.CourseLessonTaskPartFragment, container, false);

        var homework = view.FindViewById<TextView>(Resource.Id.course_lesson_task_description);

        var set = this.CreateBindingSet<LessonTaskPartFragment, CourseLessonItem>();

        set.Bind(homework)
            .For(x => x.Text)
            .To(vm => vm.HomeworkDescription);

        set.Apply();

        return view;
    }
}

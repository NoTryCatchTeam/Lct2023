using Android.OS;
using Android.Views;
using Google.Android.Material.Card;
using Lct2023.Android.Activities;
using Lct2023.Android.Fragments.MainTabs;
using Lct2023.Definitions.VmResult;
using Lct2023.ViewModels;
using MvvmCross.Binding.ValueConverters;
using MvvmCross.Commands;
using MvvmCross.Platforms.Android.Binding;

namespace Lct2023.Android.Fragments
{
    public abstract class BaseMainTabFragment<TViewModel> : BaseFragment<TViewModel>
        where TViewModel : BaseMainTabViewModel
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            var set = CreateBindingSet();

            set.Bind(view.FindViewById<MaterialCardView>(Resource.Id.toolbar_image_container))
                .For(v => v.BindClick())
                .To(vm => vm.GoToProfileCommand)
                .WithConversion<MvxCommandParameterValueConverter>(OnProfileResult);

            set.Apply();

            return view;
        }

        private void OnProfileResult(ProfileResult result)
        {
            var activity = (MainTabbedActivity)Activity;

            switch (result?.ResultType)
            {
                case Lct2023.Definitions.Enums.ProfileResultType.Courses when result.CourseIndex.HasValue:
                    activity.NavigateToPosition(1);
                    if (Activity.SupportFragmentManager?.FindFragmentByTag("f1") is not CoursesFragment course1Fragment)
                    {
                        return;
                    }

                    course1Fragment.FocusOnCourse(result.CourseIndex.Value);
                    break;
                case Lct2023.Definitions.Enums.ProfileResultType.Courses:
                    activity.NavigateToPosition(1);
                    break;
            }
        }
    }
}

using Android.OS;
using Android.Views;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.Core.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.ViewPager2.Widget;
using Google.Android.Material.Tabs;
using Lct2023.Android.Adapters;
using Lct2023.Android.Decorations;
using Lct2023.Android.Helpers;
using Lct2023.ViewModels.Courses;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace Lct2023.Android.Fragments.MainTabs;

[MvxFragmentPresentation]
public class CoursesFragment : BaseFragment<CoursesViewModel>
{
    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        var view = base.OnCreateView(inflater, container, savedInstanceState);

        Toolbar.Title = "Курсы";

        var parent = view.FindViewById<ConstraintLayout>(Resource.Id.courses_layout);
        var searchLayout = view.FindViewById<ConstraintLayout>(Resource.Id.courses_search_layout);
        var scroll = view.FindViewById<NestedScrollView>(Resource.Id.courses_scroll);
        var bannersList = view.FindViewById<ViewPager2>(Resource.Id.courses_banners_list);
        var bannersListIndicator = view.FindViewById<TabLayout>(Resource.Id.courses_banners_list_indicator);
        var coursesList = view.FindViewById<MvxRecyclerView>(Resource.Id.courses_list);

        _ = new ScrollWithSearchLayoutMediator(parent, scroll, searchLayout);

        var bannersAdapter = new CoursesBannersAdapter((IMvxAndroidBindingContext)BindingContext)
        {
            ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.courses_banner_item),
        };

        bannersList.Adapter = bannersAdapter;

        new TabLayoutMediator(
                bannersListIndicator,
                bannersList,
                true,
                true,
                new DefaultTabConfigurationStrategy((tab, _) =>
                {
                    tab.View.Clickable = false;
                }))
            .Attach();

        var coursesAdapter = new CoursesGroupsListAdapter((IMvxAndroidBindingContext)BindingContext, ViewModel.CourseTapCommand)
        {
            ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.courses_list_item),
        };

        coursesList.SetLayoutManager(new MvxGuardedLinearLayoutManager(Activity) { Orientation = LinearLayoutManager.Vertical });
        coursesList.SetAdapter(coursesAdapter);
        coursesList.AddItemDecoration(new ItemSeparateDecoration(DimensUtils.DpToPx(Activity, 8), LinearLayoutManager.Vertical));
        coursesList.HasFixedSize = false;

        if (coursesList.GetItemAnimator() is SimpleItemAnimator simpleItemAnimator)
        {
            simpleItemAnimator.SupportsChangeAnimations = false;
        }

        var set = CreateBindingSet();

        set.Bind(bannersAdapter)
            .For(x => x.ItemsSource)
            .To(vm => vm.BannersCollection);

        set.Bind(coursesAdapter)
            .For(x => x.ItemsSource)
            .To(vm => vm.CoursesGroupsCollection);

        set.Apply();

        return view;
    }

    protected override int GetLayoutId() => Resource.Layout.CoursesFragment;
}
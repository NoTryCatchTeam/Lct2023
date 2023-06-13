using System;
using System.Threading.Tasks;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.Core.Content.Resources;
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
public class CoursesFragment : BaseMainTabFragment<CoursesViewModel>
{
    private ConstraintLayout _parent;
    private NestedScrollView _scroll;
    private MvxRecyclerView _coursesList;

    public void FocusOnCourse(int index)
    {
        if (_coursesList.FindViewHolderForAdapterPosition(index) is not CoursesGroupsListAdapter.CourseGroupViewHolder courseGroupViewHolder)
        {
            return;
        }

        courseGroupViewHolder.Open();

        Task.Delay(100)
            .ContinueWith(_ =>
            {
                var courseGroupRect = new Rect();
                courseGroupViewHolder.ItemView.GetDrawingRect(courseGroupRect);
                _scroll.OffsetDescendantRectToMyCoords(courseGroupViewHolder.ItemView, courseGroupRect);

                Activity.RunOnUiThread(() => _scroll.SmoothScrollTo(0, courseGroupRect.Top - _parent.MeasuredHeight / 3, 300));
            });
    }

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        var view = base.OnCreateView(inflater, container, savedInstanceState);

        Toolbar.Title = "Курсы";

        _parent = view.FindViewById<ConstraintLayout>(Resource.Id.courses_layout);
        var searchLayout = view.FindViewById<ConstraintLayout>(Resource.Id.courses_search_layout);
        _scroll = view.FindViewById<NestedScrollView>(Resource.Id.courses_scroll);
        var bannersList = view.FindViewById<ViewPager2>(Resource.Id.courses_banners_list);
        var bannersListIndicator = view.FindViewById<TabLayout>(Resource.Id.courses_banners_list_indicator);
        var ratingCounter = view.FindViewById<TextView>(Resource.Id.courses_stats_badge_counter);
        var statsExplanation = view.FindViewById<TextView>(Resource.Id.courses_stats_open_explanation);
        _coursesList = view.FindViewById<MvxRecyclerView>(Resource.Id.courses_list);

        _ = new ScrollWithOverlayViewMediator(_parent, _scroll, searchLayout);

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

        statsExplanation.TextFormatted = GetStatsExplanationString();

        var coursesAdapter = new CoursesGroupsListAdapter((IMvxAndroidBindingContext)BindingContext, ViewModel.CourseTapCommand)
        {
            ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.courses_list_item),
        };

        _coursesList.SetLayoutManager(new MvxGuardedLinearLayoutManager(Activity) { Orientation = LinearLayoutManager.Vertical });
        _coursesList.SetAdapter(coursesAdapter);
        _coursesList.AddItemDecoration(new ItemSeparateDecoration(DimensUtils.DpToPx(Activity, 8), LinearLayoutManager.Vertical));
        _coursesList.HasFixedSize = false;

        if (_coursesList.GetItemAnimator() is SimpleItemAnimator simpleItemAnimator)
        {
            simpleItemAnimator.SupportsChangeAnimations = false;
        }

        var set = CreateBindingSet();

        set.Bind(bannersAdapter)
            .For(x => x.ItemsSource)
            .To(vm => vm.BannersCollection);

        set.Bind(ratingCounter)
            .For(x => x.Text)
            .To(vm => vm.UserContext.User.Rating);

        set.Bind(coursesAdapter)
            .For(x => x.ItemsSource)
            .To(vm => vm.CoursesGroupsCollection);

        set.Apply();

        return view;
    }

    protected override int GetLayoutId() => Resource.Layout.CoursesFragment;

    private SpannableStringBuilder GetStatsExplanationString()
    {
        var spanText = "7 баллов";
        var text = $"Наберите еще {spanText}, чтобы открыть дополнительный бонусный материал.";
        var spanIndex = text.IndexOf(spanText, StringComparison.InvariantCulture);

        var counterText = new SpannableStringBuilder(text);

        counterText.SetSpan(
            new StyleSpan(Resources.GetFont(Resource.Font.roboto_medium).Style),
            spanIndex,
            spanIndex + spanText.Length,
            SpanTypes.ExclusiveExclusive);

        counterText.SetSpan(
            new ForegroundColorSpan(Resources.GetColor(Resource.Color.accent, null)),
            spanIndex,
            spanIndex + spanText.Length,
            SpanTypes.ExclusiveExclusive);

        return counterText;
    }
}

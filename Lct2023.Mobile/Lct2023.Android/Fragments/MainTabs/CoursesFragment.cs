using System;
using System.Collections.Generic;
using System.Linq;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.Core.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.ViewPager2.Widget;
using Google.Android.Material.Card;
using Google.Android.Material.Tabs;
using Lct2023.Android.Adapters;
using Lct2023.Android.Decorations;
using Lct2023.Android.Helpers;
using Lct2023.Converters;
using Lct2023.ViewModels.Courses;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.Target;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace Lct2023.Android.Fragments.MainTabs;

[MvxFragmentPresentation]
public class CoursesFragment : BaseFragment<CoursesViewModel>
{
    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        var view = base.OnCreateView(inflater, container, savedInstanceState);

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

        var coursesAdapter = new CoursesGroupsListAdapter((IMvxAndroidBindingContext)BindingContext)
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

public class CoursesGroupsListAdapter : BaseRecyclerViewAdapter<CourseGroupItem, CoursesGroupsListAdapter.CourseGroupViewHolder>
{
    public CoursesGroupsListAdapter(IMvxAndroidBindingContext bindingContext)
        : base(bindingContext)
    {
    }

    protected override Func<View, IMvxAndroidBindingContext, CourseGroupViewHolder> BindableViewHolderCreator =>
        (v, c) => new CourseGroupViewHolder(v, c);

    public class CourseGroupViewHolder : BaseViewHolder
    {
        private readonly MvxRecyclerView _coursesList;
        private readonly ImageView _chevron;

        private CoursesListAdapter _coursesAdapter;

        public CourseGroupViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
            var image = itemView.FindViewById<ImageView>(Resource.Id.courses_list_item_image);
            var title = itemView.FindViewById<TextView>(Resource.Id.courses_list_item_title);
            var availability = itemView.FindViewById<TextView>(Resource.Id.courses_list_item_availability);
            _chevron = itemView.FindViewById<ImageView>(Resource.Id.courses_list_item_chevron);
            _coursesList = itemView.FindViewById<MvxRecyclerView>(Resource.Id.courses_list_item_list);
            _coursesList.Visibility = ViewStates.Gone;

            this.DelayBind(() =>
            {
                var set = CreateBindingSet();

                set.Bind(image)
                    .For(nameof(MvxImageViewResourceNameTargetBinding))
                    .To(vm => vm.MajorType)
                    .WithConversion(new AnyExpressionConverter<CourseMajorType, int>(x => x switch
                    {
                        CourseMajorType.Guitar => Resource.Drawable.image_course_guitar,
                        CourseMajorType.FrenchHorn => Resource.Drawable.image_course_french_horn,
                        CourseMajorType.Drums => Resource.Drawable.image_course_drums,
                        _ => Resource.Drawable.image_course_guitar,
                    }));

                set.Bind(title)
                    .For(x => x.Text)
                    .To(vm => vm.Major);

                set.Bind(availability)
                    .For(x => x.Text)
                    .To(vm => vm.Courses)
                    .WithConversion(new AnyExpressionConverter<IEnumerable<CourseItem>, string>(x => $"Доступно {x.Count()} курса"));

                set.Apply();
            });
        }

        public override void Bind()
        {
            base.Bind();

            if (_coursesAdapter != null)
            {
                return;
            }

            _coursesAdapter = new CoursesListAdapter((IMvxAndroidBindingContext)BindingContext)
            {
                ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.courses_expanded_list_item),
            };

            _coursesList.SetLayoutManager(new MvxGuardedLinearLayoutManager(_coursesList.Context) { Orientation = LinearLayoutManager.Vertical });
            _coursesList.SetAdapter(_coursesAdapter);
            _coursesList.AddItemDecoration(
                new ColoredDividerItemDecoration(_coursesList.Context, LinearLayoutManager.Vertical)
                {
                    Drawable = _coursesList.Context.GetDrawable(Resource.Drawable.simple_list_item_decorator),
                    Padding = new Rect(DimensUtils.DpToPx(_coursesList.Context, 16), 0, DimensUtils.DpToPx(_coursesList.Context, 16), 0),
                });

            var set = CreateBindingSet();

            set.Bind(_coursesAdapter)
                .For(x => x.ItemsSource)
                .To(vm => vm.Courses);

            set.Apply();
        }

        protected override void OnItemViewClick(object sender, EventArgs e)
        {
            base.OnItemViewClick(sender, e);

            var shouldOpen = _coursesList.Visibility == ViewStates.Gone;

            _coursesList.Visibility = shouldOpen ? ViewStates.Visible : ViewStates.Gone;
            _chevron.Rotation = shouldOpen ? 90 : 0;
        }
    }
}

public class CoursesListAdapter : BaseRecyclerViewAdapter<CourseItem, CoursesListAdapter.CourseViewHolder>
{
    public CoursesListAdapter(IMvxAndroidBindingContext bindingContext)
        : base(bindingContext)
    {
    }

    protected override Func<View, IMvxAndroidBindingContext, CourseViewHolder> BindableViewHolderCreator =>
        (v, c) => new CourseViewHolder(v, c);

    public class CourseViewHolder : BaseViewHolder
    {
        private readonly MvxRecyclerView _coursesList;

        private CourseTagsListAdapter _tagsListAdapter;

        public CourseViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
            var title = itemView.FindViewById<TextView>(Resource.Id.courses_expanded_list_item_title);
            _coursesList = itemView.FindViewById<MvxRecyclerView>(Resource.Id.courses_expanded_list_item_list);

            this.DelayBind(() =>
            {
                var set = CreateBindingSet();

                set.Bind(title)
                    .For(x => x.Text)
                    .To(vm => vm.Title);

                set.Apply();
            });
        }

        public override void Bind()
        {
            base.Bind();

            if (_tagsListAdapter != null)
            {
                return;
            }

            _tagsListAdapter = new CourseTagsListAdapter((IMvxAndroidBindingContext)BindingContext)
            {
                ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.courses_tags_list_item),
            };

            _coursesList.SetLayoutManager(new MvxGuardedLinearLayoutManager(_coursesList.Context) { Orientation = LinearLayoutManager.Horizontal });
            _coursesList.SetAdapter(_tagsListAdapter);
            _coursesList.AddItemDecoration(new ItemSeparateDecoration(DimensUtils.DpToPx(_coursesList.Context, 8), LinearLayoutManager.Horizontal));

            var set = CreateBindingSet();

            set.Bind(_tagsListAdapter)
                .For(x => x.ItemsSource)
                .To(vm => vm.Tags);

            set.Apply();
        }
    }
}

public class CourseTagsListAdapter : BaseRecyclerViewAdapter<CourseTagItem, CourseTagsListAdapter.CourseTagViewHolder>
{
    public CourseTagsListAdapter(IMvxAndroidBindingContext bindingContext)
        : base(bindingContext)
    {
    }

    protected override Func<View, IMvxAndroidBindingContext, CourseTagViewHolder> BindableViewHolderCreator =>
        (v, c) => new CourseTagViewHolder(v, c);

    public class CourseTagViewHolder : BaseViewHolder
    {
        public CourseTagViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
            var layout = itemView.FindViewById<MaterialCardView>(Resource.Id.courses_tags_list_item_layout);
            var title = itemView.FindViewById<TextView>(Resource.Id.courses_tags_list_item_title);

            this.DelayBind(() =>
            {
                var set = CreateBindingSet();

                set.Bind(title)
                    .For(x => x.Text)
                    .To(vm => vm.Title);

                set.Apply();
            });
        }
    }
}

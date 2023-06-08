using Android.OS;
using Android.Views;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.ViewPager2.Adapter;
using AndroidX.ViewPager2.Widget;
using DataModel.Definitions.Enums;
using Google.Android.Material.Button;
using Google.Android.Material.Card;
using Lct2023.Android.Activities;
using Lct2023.Android.Adapters;
using Lct2023.Android.Decorations;
using Lct2023.Android.Definitions.Extensions;
using Lct2023.Android.Helpers;
using Lct2023.Android.Listeners;
using Lct2023.ViewModels.Main;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using Lct2023.Converters;
using Lct2023.Definitions.Internals;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;
using MvvmCross.Platforms.Android.Binding;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace Lct2023.Android.Fragments.MainTabs;

[MvxFragmentPresentation]
public class MainFragment : BaseFragment<MainViewModel>
{
    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        var view = base.OnCreateView(inflater, container, savedInstanceState);

        var activity = (MainTabbedActivity)Activity;
        var stories = view.FindViewById<MvxRecyclerView>(Resource.Id.main_stories);

        (ConstraintLayout Layout, MaterialButton Button) profTest = (
            view.FindViewById<ConstraintLayout>(Resource.Id.main_proftest_layout),
            view.FindViewById<MaterialButton>(Resource.Id.main_proftest_button));

        var allCourses = view.FindViewById<MaterialButton>(Resource.Id.main_all_courses_button);
        var course1 = view.FindViewById<MaterialCardView>(Resource.Id.main_course_1_layout);
        var course2 = view.FindViewById<MaterialCardView>(Resource.Id.main_course_2_layout);
        var allEvents = view.FindViewById<MaterialButton>(Resource.Id.main_all_events_button);
        var events = view.FindViewById<MvxRecyclerView>(Resource.Id.main_events_collection);

        var clickListener = new DefaultClickListener(v =>
        {
            switch (v.Id)
            {
                case Resource.Id.main_all_courses_button:
                    activity.NavigateToPosition(1);

                    break;
                case Resource.Id.main_course_1_layout:
                    activity.NavigateToPosition(1);

                    if (Activity.SupportFragmentManager?.FindFragmentByTag("f1") is not CoursesFragment course1Fragment)
                    {
                        return;
                    }

                    course1Fragment.FocusOnCourse(0);

                    break;
                case Resource.Id.main_course_2_layout:
                    activity.NavigateToPosition(1);

                    if (Activity.SupportFragmentManager?.FindFragmentByTag("f1") is not CoursesFragment course2Fragment)
                    {
                        return;
                    }

                    course2Fragment.FocusOnCourse(1);

                    break;
                case Resource.Id.main_all_events_button:
                    activity.NavigateToPosition(4);

                    if (Activity.SupportFragmentManager?.FindFragmentByTag("f4") is not MapFragment mapFragment)
                    {
                        return;
                    }

                    mapFragment.ViewModel.UpdatePlaces(LocationType.Event);

                    break;
            }
        });

        var storiesAdapter = new MainStoriesAdapter((IMvxAndroidBindingContext)BindingContext)
        {
            ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.StoryCard),
        };

        stories.SetLayoutManager(new MvxGuardedLinearLayoutManager(Activity) { Orientation = LinearLayoutManager.Horizontal });
        stories.SetAdapter(storiesAdapter);
        stories.AddItemDecoration(new ItemSeparateDecoration(DimensUtils.DpToPx(Activity, 4), LinearLayoutManager.Horizontal));

        allCourses.SetOnClickListener(clickListener);
        course1.SetOnClickListener(clickListener);
        course2.SetOnClickListener(clickListener);
        allEvents.SetOnClickListener(clickListener);

        var eventsAdapter = new MainEventsAdapter((IMvxAndroidBindingContext)BindingContext)
        {
            ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.main_events_list_item),
        };

        events.SetLayoutManager(new MvxGuardedLinearLayoutManager(Activity) { Orientation = LinearLayoutManager.Horizontal });
        events.SetAdapter(eventsAdapter);
        events.AddItemDecoration(new ItemSeparateDecoration(DimensUtils.DpToPx(Activity, 8), LinearLayoutManager.Horizontal));

        var set = CreateBindingSet();

        set.Bind(storiesAdapter)
            .For(v => v.ItemsSource)
            .To(vm => vm.StoryCards);

        set.Bind(profTest.Layout)
            .For(x => x.BindVisible())
            .To(vm => vm.UserContext.User)
            .WithConversion(new AnyExpressionConverter<User, bool>(x => x?.IsProfTestFinished == false));

        set.Bind(profTest.Button)
            .For(x => x.BindClick())
            .To(vm => vm.StartProfTestCommand);

        set.Bind(eventsAdapter)
            .For(v => v.ItemsSource)
            .To(vm => vm.EventsCollection);

        set.Apply();

        return view;
    }

    protected override int GetLayoutId() => Resource.Layout.MainFragment;
}

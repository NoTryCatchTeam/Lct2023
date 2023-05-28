using Android.OS;
using Android.Views;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.Button;
using Lct2023.Android.Adapters;
using Lct2023.Android.Decorations;
using Lct2023.Android.Helpers;
using Lct2023.ViewModels.Main;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using Lct2023.Commons.Extensions;
using Lct2023.Converters;
using Lct2023.Definitions.Internals;
using Lct2023.Services;
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

        var stories = view.FindViewById<MvxRecyclerView>(Resource.Id.main_stories);

        (ConstraintLayout Layout, MaterialButton Button) profTest = (
            view.FindViewById<ConstraintLayout>(Resource.Id.main_proftest_layout),
            view.FindViewById<MaterialButton>(Resource.Id.main_proftest_button));

        var events = view.FindViewById<MvxRecyclerView>(Resource.Id.main_events_collection);

        var storiesAdapter = new MainStoriesAdapter((IMvxAndroidBindingContext)BindingContext)
        {
            ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.StoryCard),
        };

        stories.SetLayoutManager(new MvxGuardedLinearLayoutManager(Activity) { Orientation = LinearLayoutManager.Horizontal });
        stories.SetAdapter(storiesAdapter);
        stories.AddItemDecoration(new ItemSeparateDecoration(DimensUtils.DpToPx(Activity, 4), LinearLayoutManager.Horizontal));

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

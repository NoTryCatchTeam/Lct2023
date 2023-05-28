using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Lct2023.Android.Adapters;
using Lct2023.Android.Decorations;
using Lct2023.Android.Helpers;
using Lct2023.ViewModels.Main;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using Lct2023.Commons.Extensions;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using Square.Picasso;

namespace Lct2023.Android.Fragments.MainTabs;

[MvxFragmentPresentation]
public class MainFragment : BaseFragment<MainViewModel>
{
    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        var view = base.OnCreateView(inflater, container, savedInstanceState);

        var stories = view.FindViewById<MvxRecyclerView>(Resource.Id.main_stories);
        var pointsTextView = view.FindViewById<TextView>(Resource.Id.main_points_text);
        var rankingTextView = view.FindViewById<TextView>(Resource.Id.main_ranking_text);

        var storiesAdapter = new MainStoriesAdapter((IMvxAndroidBindingContext)BindingContext)
        {
            ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.StoryCard),
        };

        stories.SetLayoutManager(new MvxGuardedLinearLayoutManager(Activity) { Orientation = LinearLayoutManager.Horizontal });
        stories.SetAdapter(storiesAdapter);
        stories.AddItemDecoration(new ItemSeparateDecoration(DimensUtils.DpToPx(Activity, 4), LinearLayoutManager.Horizontal));

        var points = 724;
        var position = 3;

        pointsTextView.Text = $"{points} {points.FormatEnding("балл", "балла", "баллов")}";
        rankingTextView.Text = $"{position} место";

        var set = CreateBindingSet();

        set.Bind(stories)
            .For(v => v.ItemsSource)
            .To(vm => vm.StoryCards);

        set.Apply();

        return view;
    }

    protected override int GetLayoutId() => Resource.Layout.MainFragment;
}
using Android.OS;
using Android.Views;
using Android.Widget;
using Lct2023.ViewModels.Main;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.Views;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using Lct2023.Commons.Extensions;
using Square.Picasso;

namespace Lct2023.Android.Fragments.MainTabs;

[MvxFragmentPresentation]
public class MainFragment : BaseFragment<MainViewModel>
{
    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        var view = base.OnCreateView(inflater, container, savedInstanceState);

        var storiesLayout = view.FindViewById<MvxLinearLayout>(Resource.Id.stories_layout);
        var pointsTextView = view.FindViewById<TextView>(Resource.Id.points_text);
        var rankingTextView = view.FindViewById<TextView>(Resource.Id.ranking_text);
        var avatarImageButton = view.FindViewById<ImageView>(Resource.Id.toolbar_image);

        storiesLayout.ItemTemplateId = Resource.Layout.StoryCard;

        var points = 724;
        var position = 3;

        pointsTextView.Text = $"{points} {points.FormatEnding("балл", "балла", "баллов")}";
        rankingTextView.Text = $"{position} место";

        Picasso.Get()
            .Load(ViewModel.Image)
            .Into(avatarImageButton);

        var set = CreateBindingSet();

        set
            .Bind(storiesLayout)
            .For(v => v.ItemsSource)
            .To(vm => vm.StoryCards);

        set.Apply();

        return view;
    }

    protected override int GetLayoutId() => Resource.Layout.MainFragment;
}

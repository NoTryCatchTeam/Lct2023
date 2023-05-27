using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.Button;
using Google.Android.Material.Card;
using Google.Android.Material.TextField;
using Lct2023.Android.Adapters;
using Lct2023.Android.Bindings;
using Lct2023.Android.Decorations;
using Lct2023.Android.Helpers;
using Lct2023.Converters;
using Lct2023.ViewModels.Feed;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using Square.Picasso;

namespace Lct2023.Android.Fragments.MainTabs;

[MvxFragmentPresentation]
public class FeedFragment : BaseFragment<FeedViewModel>, View.IOnClickListener
{
    private MaterialCardView _filtersBottomSheet;
    private BottomSheetBehavior _filtersBottomSheetBehavior;
    
    protected override int GetLayoutId() => Resource.Layout.FeedFragment;

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        var view = base.OnCreateView(inflater, container, savedInstanceState);
        
        view.FindViewById<TextView>(Resource.Id.title).Text = "Лента";
        var feedSearchEditText = view.FindViewById<TextInputEditText>(Resource.Id.feed_search_edit_text);
        var avatarImageButton = view.FindViewById<ImageView>(Resource.Id.toolbar_image);
        var filtersButton = view.FindViewById<MaterialButton>(Resource.Id.feed_filters_button);
        _filtersBottomSheet = view.FindViewById<MaterialCardView>(Resource.Id.filters_bottom_sheet);
        _filtersBottomSheetBehavior = BottomSheetBehavior.From(_filtersBottomSheet);
        var feedRecycler = view.FindViewById<MvxRecyclerView>(Resource.Id.feed_recycler);
        var filtersRecycler = view.FindViewById<MvxRecyclerView>(Resource.Id.feed_filters_recycle);
        var filtersCloseBsButton = view.FindViewById<MaterialButton>(Resource.Id.feed_filters_close_bs_button);
        var clearFiltersButton = view.FindViewById<MaterialButton>(Resource.Id.feed_clear_filters_button);
        var applyFiltersButton = view.FindViewById<MaterialButton>(Resource.Id.feed_apply_filters_button);
        
        var vertical16dpItemSpacingDecoration = new ItemSeparateDecoration(DimensUtils.DpToPx(Activity, 16), LinearLayoutManager.Vertical);

        var feedFiltersAdapter = new FeedFiltersGroupsListAdapter((IMvxAndroidBindingContext)BindingContext)
        {
            ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.MapFiltersGroupItemView),
        };

        filtersRecycler.SetLayoutManager(new MvxGuardedLinearLayoutManager(Context) { Orientation = LinearLayoutManager.Vertical });
        filtersRecycler.SetAdapter(feedFiltersAdapter);
        filtersRecycler.AddItemDecoration(vertical16dpItemSpacingDecoration);
        
        foreach (var button in new [] { filtersButton, filtersCloseBsButton, applyFiltersButton, clearFiltersButton })
        {
            button.SetOnClickListener(this);
        }
        
        Picasso.Get()
            .Load(ViewModel.Image)
            .Into(avatarImageButton);
        
        var set = CreateBindingSet();

        set
            .Bind(feedSearchEditText)
            .For(v => v.Text)
            .To(vm => vm.SearchText);
        
        set
            .Bind(filtersButton)
            .For(nameof(ButtonIconResourceBinding))
            .To(vm => vm.SelectedFilters)
            .WithConversion(new AnyExpressionConverter<object, int>(filters => filters == null ? Resource.Drawable.ic_filters : Resource.Drawable.ic_filters_selected));

        set
            .Bind(feedRecycler)
            .For(v => v.ItemsSource)
            .To(vm => vm.Items);
        
        set
            .Bind(filtersRecycler)
            .For(v => v.ItemsSource)
            .To(vm => vm.FilterGroups);
        
        set.Apply();

        return view;
    }

    public void OnClick(View view)
    {
        switch (view.Id)
        {
            case Resource.Id.feed_apply_filters_button:
                _filtersBottomSheetBehavior.State = BottomSheetBehavior.StateHidden;
                ViewModel.ApplyFilters();
                break;
            case Resource.Id.feed_clear_filters_button:
                _filtersBottomSheetBehavior.State = BottomSheetBehavior.StateHidden;
                ViewModel.ResetFilters();
                break;
            case Resource.Id.feed_filters_button:
                _filtersBottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;
                break;
            case Resource.Id.feed_filters_close_bs_button:
                _filtersBottomSheetBehavior.State = BottomSheetBehavior.StateHidden;
                break;
        }
    }
}

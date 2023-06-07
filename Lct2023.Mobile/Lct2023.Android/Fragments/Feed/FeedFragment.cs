using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.Button;
using Google.Android.Material.Card;
using Google.Android.Material.TextField;
using Lct2023.Android.Adapters;
using Lct2023.Android.Bindings;
using Lct2023.Android.Callbacks;
using Lct2023.Android.Decorations;
using Lct2023.Android.Helpers;
using Lct2023.Android.Listeners;
using Lct2023.Android.Presenters;
using Lct2023.Android.Views;
using Lct2023.Converters;
using Lct2023.Definitions.Enums;
using Lct2023.ViewModels.Feed;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;
using MvvmCross.Platforms.Android.Binding;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace Lct2023.Android.Fragments.MainTabs;

[MvxFragmentAdapterChildItemPresentationAttribute(
    RootPosition = 2,
    FragmentHostViewType = typeof(MainFeedFragment),
    FragmentContentId = Resource.Id.main_feed_container,
    AddToBackStack = true)]
public class FeedFragment : BaseFragment<FeedViewModel>, View.IOnClickListener
{
    private const float MAX_DIM_ALPHA = 0.5f;

    private MaterialCardView _filtersBottomSheet;
    private BottomSheetBehavior _filtersBottomSheetBehavior;
    
    protected override int GetLayoutId() => Resource.Layout.FeedFragment;

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        var view = base.OnCreateView(inflater, container, savedInstanceState);

        Toolbar.Title = "Лента";
        var feedSearchEditText = view.FindViewById<TextInputEditText>(Resource.Id.feed_search_edit_text);
        var filtersButton = view.FindViewById<MaterialButton>(Resource.Id.feed_filters_button);
        _filtersBottomSheet = view.FindViewById<MaterialCardView>(Resource.Id.feed_filters_bottom_sheet);
        _filtersBottomSheetBehavior = BottomSheetBehavior.From(_filtersBottomSheet);
        var dimView = view.FindViewById(Resource.Id.feed_dim);
        var feedRecycler = view.FindViewById<MvxRecyclerView>(Resource.Id.feed_recycle);
        var filtersRecycler = view.FindViewById<MvxRecyclerView>(Resource.Id.feed_filters_recycle);
        var filtersCloseBsButton = view.FindViewById<MaterialButton>(Resource.Id.feed_filters_close_bs_button);
        var clearFiltersButton = view.FindViewById<MaterialButton>(Resource.Id.feed_clear_filters_button);
        var applyFiltersButton = view.FindViewById<MaterialButton>(Resource.Id.feed_apply_filters_button);

        var vertical16dpItemSpacingDecoration = new ItemSeparateDecoration(DimensUtils.DpToPx(Activity, 16), LinearLayoutManager.Vertical);

        var feedFiltersAdapter = new FeedFiltersGroupsListAdapter((IMvxAndroidBindingContext)BindingContext)
        {
            ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.FeedFiltersGroupItemView),
        };

        dimView.SetOnTouchListener(new DefaultOnTouchListener((v, e) =>
        {
            var isExpanded = _filtersBottomSheetBehavior.State == BottomSheetBehavior.StateExpanded;
            if (isExpanded)
            {
                _filtersBottomSheetBehavior.State = BottomSheetBehavior.StateHidden;
            }

            return isExpanded;
        }));

        var bottomSheetCallback = new DefaultBottomSheetCallback(
            (v, s) =>
            {
                dimView.Alpha = (s > MAX_DIM_ALPHA) switch
                {
                    true => MAX_DIM_ALPHA,
                    _ => s,
                };
            });

        _filtersBottomSheetBehavior.AddBottomSheetCallback(bottomSheetCallback);


        filtersRecycler.SetLayoutManager(new MvxGuardedLinearLayoutManager(Context) { Orientation = LinearLayoutManager.Vertical });
        filtersRecycler.SetAdapter(feedFiltersAdapter);
        filtersRecycler.AddItemDecoration(vertical16dpItemSpacingDecoration);

        var vertical8dpItemSpacingDecoration = new ItemSeparateDecoration(DimensUtils.DpToPx(Activity, 8), LinearLayoutManager.Vertical);

        var feedAdapter = new FeedListAdapter<FeedViewModel>((IMvxAndroidBindingContext)BindingContext, ViewModel, (CreateFeedRecyclerHeader, BindFeedRecyclerHeader))
        {
            ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.FeedItemView),
        };

        var feedLayoutManager = new MvxGuardedLinearLayoutManager(Context) { Orientation = LinearLayoutManager.Vertical };
        feedRecycler.SetLayoutManager(feedLayoutManager);
        feedRecycler.SetAdapter(feedAdapter);
        feedRecycler.AddItemDecoration(vertical8dpItemSpacingDecoration);
        var feedScrollListener = new RecyclerPaginationListener(feedLayoutManager);
        feedRecycler.AddOnScrollListener(feedScrollListener);

        foreach (var button in new [] { filtersButton, filtersCloseBsButton, applyFiltersButton, clearFiltersButton })
        {
            button.SetOnClickListener(this);
        }
        
        var set = CreateBindingSet();

        set
            .Bind(feedSearchEditText)
            .For(v => v.Text)
            .To(vm => vm.SearchText);
        
        set
            .Bind(filtersButton)
            .For(nameof(ButtonIconResourceBinding))
            .To(vm => vm.SelectedFilters)
            .WithConversion(new AnyExpressionConverter<ObservableCollection<(FeedFilterGroupType FilterGroupType, string[] Items)>, int>(filters => filters?.Any() == true ?  Resource.Drawable.ic_filters_selected : Resource.Drawable.ic_filters));
        
        set
            .Bind(view.FindViewById(Resource.Id.feed_loading))
            .For(v => v.BindVisible())
            .To(vm => vm.State)
            .WithConversion(new AnyExpressionConverter<State, bool>(state => state is State.Loading or State.MinorLoading));

        set
            .Bind(view.FindViewById(Resource.Id.feed_no_data))
            .For(v => v.BindVisible())
            .To(vm => vm.State)
            .WithConversion(new AnyExpressionConverter<State, bool>(state => state == State.NoData));

        set
            .Bind(filtersRecycler)
            .For(v => v.ItemsSource)
            .To(vm => vm.FilterGroups);

        set
            .Bind(feedRecycler)
            .For(v => v.ItemsSource)
            .To(vm => vm.Items);

        set
            .Bind(feedAdapter)
            .For(v => v.Items)
            .To(vm => vm.Items);

        set
            .Bind(feedAdapter)
            .For(v => v.IsLoadingMore)
            .To(vm => vm.IsLoadingMore);

        set.Bind(feedScrollListener)
            .For(x => x.LoadMoreCommand)
            .To(vm => vm.LoadMoreCommand);

        set.Bind(feedScrollListener)
            .For(x => x.LoadingOffset)
            .To(vm => vm.LoadingOffset);

        set
            .Bind(feedScrollListener)
            .For(v => v.IsLoadingMore)
            .To(vm => vm.IsLoadingMore);

        set.Apply();

        return view;

        View CreateFeedRecyclerHeader() => LayoutInflater.Inflate(Resource.Layout.FeedArtDirectionsHeaderItemView, null, false);

        static void BindFeedRecyclerHeader(View view, IMvxAndroidBindingContext bindingContext, MvxFluentBindingDescriptionSet<FeedListAdapter<FeedViewModel>.FeedHeaderItemViewHolder, FeedViewModel> set)
        {
            var artDirectionsRecycler = view.FindViewById<MvxRecyclerView>(Resource.Id.feed_art_directions_collection);

            var horizontal8dpItemSpacingDecoration = new ItemSeparateDecoration(DimensUtils.DpToPx(view.Context, 8), LinearLayoutManager.Horizontal);

            var feedArtDirectionsAdapter = new FeedArtDirectionsAdapter(bindingContext)
            {
                ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.FeedArtDirectionItemView),
            };

            artDirectionsRecycler.SetLayoutManager(new MvxGuardedLinearLayoutManager(view.Context) { Orientation = LinearLayoutManager.Horizontal });
            artDirectionsRecycler.SetAdapter(feedArtDirectionsAdapter);
            artDirectionsRecycler.AddItemDecoration(horizontal8dpItemSpacingDecoration);

            set
                .Bind(artDirectionsRecycler)
                .For(v => v.ItemsSource)
                .To(vm => vm.ArtDirections);

            set
                .Bind(artDirectionsRecycler)
                .For(v => v.ItemClick)
                .To(vm => vm.ArtDirectionClickCommand);

            set
                .Bind(artDirectionsRecycler)
                .For(v => v.ItemLongClick)
                .To(vm => vm.ArtDirectionClickCommand);
        }
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

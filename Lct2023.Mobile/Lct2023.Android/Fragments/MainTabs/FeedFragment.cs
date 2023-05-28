using System;
using System.Collections.Generic;
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
using Lct2023.Android.Listeners;
using Lct2023.Android.Views;
using Lct2023.Converters;
using Lct2023.Definitions.Enums;
using Lct2023.ViewModels.Feed;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;
using MvvmCross.Platforms.Android.Binding;
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
        MvxRecyclerView feedRecycler = null;
        
        var view = base.OnCreateView(inflater, container, savedInstanceState);
        
        view.FindViewById<TextView>(Resource.Id.toolbar_title).Text = "Лента";
        var feedSearchEditText = view.FindViewById<TextInputEditText>(Resource.Id.feed_search_edit_text);
        var avatarImageButton = view.FindViewById<ImageView>(Resource.Id.toolbar_image);
        var filtersButton = view.FindViewById<MaterialButton>(Resource.Id.feed_filters_button);
        _filtersBottomSheet = view.FindViewById<MaterialCardView>(Resource.Id.feed_filters_bottom_sheet);
        _filtersBottomSheetBehavior = BottomSheetBehavior.From(_filtersBottomSheet);
        var feedStateContainer = view.FindViewById<StateContainer>(Resource.Id.feed_state_container);
        var filtersRecycler = view.FindViewById<MvxRecyclerView>(Resource.Id.feed_filters_recycle);
        var filtersCloseBsButton = view.FindViewById<MaterialButton>(Resource.Id.feed_filters_close_bs_button);
        var clearFiltersButton = view.FindViewById<MaterialButton>(Resource.Id.feed_clear_filters_button);
        var applyFiltersButton = view.FindViewById<MaterialButton>(Resource.Id.feed_apply_filters_button);

        feedStateContainer.States = new Dictionary<State, Func<View>>
        {
            [State.Default] = CreateFeedRecycler
        };
        
        var vertical16dpItemSpacingDecoration = new ItemSeparateDecoration(DimensUtils.DpToPx(Activity, 16), LinearLayoutManager.Vertical);

        var feedFiltersAdapter = new FeedFiltersGroupsListAdapter((IMvxAndroidBindingContext)BindingContext)
        {
            ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.FeedFiltersGroupItemView),
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
            .Bind(filtersRecycler)
            .For(v => v.ItemsSource)
            .To(vm => vm.FilterGroups);
        
        set
            .Bind(feedStateContainer)
            .For(v => v.State)
            .To(vm => vm.State);
        
        set.Apply();

        return view;

        View CreateFeedRecycler()
        {
            return feedRecycler ??= CreateInnerFeedRecycler();
            
            MvxRecyclerView CreateInnerFeedRecycler()
            {
                feedRecycler = LayoutInflater.Inflate(Resource.Layout.FeedItemsRecycler, feedStateContainer, false) as MvxRecyclerView;
                
                var vertical8dpItemSpacingDecoration = new ItemSeparateDecoration(DimensUtils.DpToPx(Activity, 8), LinearLayoutManager.Vertical);

                var feedAdapter = new FeedListAdapter((IMvxAndroidBindingContext)BindingContext)
                {
                    ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.FeedItemView),
                };

                var feedLayoutManager = new MvxGuardedLinearLayoutManager(Context) { Orientation = LinearLayoutManager.Vertical };
                feedRecycler.SetLayoutManager(feedLayoutManager);
                feedRecycler.SetAdapter(feedAdapter);
                feedRecycler.AddItemDecoration(vertical8dpItemSpacingDecoration);
                var feedScrollListener = new RecyclerPaginationListener(feedLayoutManager);
                feedRecycler.AddOnScrollListener(feedScrollListener);
                
                var innerSet = CreateBindingSet();
                
                innerSet
                    .Bind(feedRecycler)
                    .For(v => v.ItemClick)
                    .To(vm => vm.ItemClickCommand);

                innerSet
                    .Bind(feedRecycler)
                    .For(v => v.ItemLongClick)
                    .To(vm => vm.ItemClickCommand);
                
                innerSet
                    .Bind(feedRecycler)
                    .For(v => v.ItemsSource)
                    .To(vm => vm.Items);
                
                innerSet
                    .Bind(feedAdapter)
                    .For(v => v.Items)
                    .To(vm => vm.Items);

                innerSet
                    .Bind(feedAdapter)
                    .For(v => v.IsLoadingMore)
                    .To(vm => vm.IsLoadingMore);

                innerSet.Bind(feedScrollListener)
                    .For(x => x.LoadMoreCommand)
                    .To(vm => vm.LoadMoreCommand);

                innerSet.Bind(feedScrollListener)
                    .For(x => x.LoadingOffset)
                    .To(vm => vm.LoadingOffset);

                innerSet
                    .Bind(feedScrollListener)
                    .For(v => v.IsLoadingMore)
                    .To(vm => vm.IsLoadingMore);

                innerSet.Apply();

                return feedRecycler;
            }
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Activity;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.Lifecycle;
using AndroidX.RecyclerView.Widget;
using AndroidX.ViewPager.Widget;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.Button;
using Google.Android.Material.Card;
using Google.Android.Material.TextField;
using Lct2023.Android.Adapters;
using Lct2023.Android.Bindings;
using Lct2023.Android.Callbacks;
using Lct2023.Android.Decorations;
using Lct2023.Android.Fragments.MainTabs;
using Lct2023.Android.Helpers;
using Lct2023.Android.Listeners;
using Lct2023.Android.Presenters;
using Lct2023.Android.Views;
using Lct2023.Commons.Extensions;
using Lct2023.Converters;
using Lct2023.Definitions.Enums;
using Lct2023.ViewModels.Feed;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;
using MvvmCross.Platforms.Android.Binding;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using static Lct2023.Android.Adapters.FeedArtDirectionsAdapter;

namespace Lct2023.Android.Fragments.Feed
{
    [MvxFragmentAdapterChildItemPresentationAttribute(
        RootPosition = 2,
        FragmentHostViewType = typeof(MainFeedFragment),
        FragmentContentId = Resource.Id.main_feed_container,
        AddToBackStack = true)]
    public class ArtDirectionFeedFragment : BaseFragment<ArtDirectionFeedViewModel>, View.IOnClickListener
    {
        private const float MAX_DIM_ALPHA = 0.5f;

        private MaterialCardView _filtersBottomSheet;
        private BottomSheetBehavior _filtersBottomSheetBehavior;
        private DefaultBackPressedCallback _backPressedCallback;

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            _backPressedCallback?.Then(c => c.Enabled = false);
            _backPressedCallback = new DefaultBackPressedCallback(
                true,
                callback => ViewModel.NavigateBackCommand.Execute(null));

            RequireActivity().OnBackPressedDispatcher.AddCallback(_backPressedCallback);
        }

        protected override int GetLayoutId() => Resource.Layout.ArtDirectionFeedFragment;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            Toolbar.Title = "Лента";
            var feedSearchEditText = view.FindViewById<TextInputEditText>(Resource.Id.art_dir_feed_search_edit_text);
            var filtersButton = view.FindViewById<MaterialButton>(Resource.Id.art_dir_feed_filters_button);
            _filtersBottomSheet = view.FindViewById<MaterialCardView>(Resource.Id.art_dir_feed_filters_bottom_sheet);
            _filtersBottomSheetBehavior = BottomSheetBehavior.From(_filtersBottomSheet);
            var dimView = view.FindViewById(Resource.Id.art_dir_feed_dim);
            var feedRecycler = view.FindViewById<MvxRecyclerView>(Resource.Id.art_dir_feed_recycle);
            var filtersRecycler = view.FindViewById<MvxRecyclerView>(Resource.Id.art_dir_feed_filters_recycle);
            var filtersCloseBsButton = view.FindViewById<MaterialButton>(Resource.Id.art_dir_feed_filters_close_bs_button);
            var clearFiltersButton = view.FindViewById<MaterialButton>(Resource.Id.art_dir_feed_clear_filters_button);
            var applyFiltersButton = view.FindViewById<MaterialButton>(Resource.Id.art_dir_feed_apply_filters_button);

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

            var feedAdapter = new FeedListAdapter<ArtDirectionFeedViewModel>(
                (IMvxAndroidBindingContext)BindingContext,
                ViewModel,
                (CreateFeedArtDirectionDescriptionHeader, BindFeedArtDirectionDescriptionHeader),
                (CreateFeedArtDirectionSchoolsHeader, BindFeedArtDirectionSchoolsHeader))
            {
                ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.FeedItemView),
            };

            var feedLayoutManager = new MvxGuardedLinearLayoutManager(Context) { Orientation = LinearLayoutManager.Vertical };
            feedRecycler.SetLayoutManager(feedLayoutManager);
            feedRecycler.SetAdapter(feedAdapter);
            feedRecycler.AddItemDecoration(vertical8dpItemSpacingDecoration);
            var feedScrollListener = new RecyclerPaginationListener(feedLayoutManager);
            feedRecycler.AddOnScrollListener(feedScrollListener);

            foreach (var button in new[] { filtersButton, filtersCloseBsButton, applyFiltersButton, clearFiltersButton })
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
                .WithConversion(new AnyExpressionConverter<ObservableCollection<(FeedFilterGroupType FilterGroupType, string[] Items)>, int>(filters => filters?.Any() == true ? Resource.Drawable.ic_filters_selected : Resource.Drawable.ic_filters));

            set
                .Bind(view.FindViewById(Resource.Id.art_dir_feed_loading))
                .For(v => v.BindVisible())
                .To(vm => vm.State)
                .WithConversion(new AnyExpressionConverter<State, bool>(state => state is State.Loading or State.MinorLoading));

            set
                .Bind(view.FindViewById(Resource.Id.art_dir_feed_no_data))
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

            View CreateFeedArtDirectionSchoolsHeader() => LayoutInflater.Inflate(Resource.Layout.FeedArtDirectionSchoolsItemView, feedRecycler, false);

            static void BindFeedArtDirectionSchoolsHeader(View view, IMvxAndroidBindingContext bindingContext, MvxFluentBindingDescriptionSet<FeedListAdapter<ArtDirectionFeedViewModel>.FeedHeaderItemViewHolder, ArtDirectionFeedViewModel> set)
            {
                set
                    .Bind(view.FindViewById<MaterialButton>(Resource.Id.feed_art_dir_schools_header_map_button))
                    .For(v => v.BindClick())
                    .To(vm => vm.OpenMapCommand);
            }

            View CreateFeedArtDirectionDescriptionHeader() => LayoutInflater.Inflate(Resource.Layout.FeedArtDirectionDescriptionItemView, feedRecycler, false);

            static void BindFeedArtDirectionDescriptionHeader(View view, IMvxAndroidBindingContext bindingContext, MvxFluentBindingDescriptionSet<FeedListAdapter<ArtDirectionFeedViewModel>.FeedHeaderItemViewHolder, ArtDirectionFeedViewModel> set)
            {
                set
                    .Bind(view.FindViewById<TextView>(Resource.Id.feed_art_dir_desc_header_title))
                    .For(v => v.Text)
                    .To(vm => vm.SelectedArtDirection);


                set
                    .Bind(view.FindViewById<TextView>(Resource.Id.feed_art_dir_desc_header_description))
                    .For(v => v.Text)
                    .To(vm => vm.ArtDirectionFeedDescription);

                set.Bind(view.FindViewById<ImageView>(Resource.Id.feed_art_dir_desc_header_image))
                    .For(nameof(ImageViewByResIdBinding))
                    .To(vm => vm.SelectedArtDirection)
                    .WithConversion(new AnyExpressionConverter<string, int>(name => name switch
                    {
                        "Хореография" => Resource.Drawable.image_horeo_art_dir,
                        "Рисование" => Resource.Drawable.image_draw_art_dir,
                        "Театр" or "Цирк" => Resource.Drawable.image_theatre_art_dir,
                        "Музыка" => Resource.Drawable.image_music_art_dir,
                        _ => Resource.Drawable.image_music_art_dir,
                    }));

                set.Bind(view.FindViewById<MaterialCardView>(Resource.Id.feed_art_dir_desc_header_image_background))
                    .For(nameof(CardViewBackgroundColorByHexBinding))
                    .To(vm => vm.SelectedArtDirection)
                    .WithConversion(new AnyExpressionConverter<string, string>(name => name switch
                    {
                        "Хореография" => "#F6F4BF",
                        "Рисование" => "#AAC5D2",
                        "Театр" or "Цирк" => "#D09EB5",
                        "Музыка" => "#a4cf57",
                        _ => "#a4cf57",
                    }));
            }
        }

        public override void OnResume()
        {
            base.OnResume();
            _backPressedCallback.Enabled = true;
        }

        public override void OnPause()
        {
            base.OnPause();
            _backPressedCallback.Enabled = false;
        }

        public void OnClick(View view)
        {
            switch (view.Id)
            {
                case Resource.Id.art_dir_feed_apply_filters_button:
                    _filtersBottomSheetBehavior.State = BottomSheetBehavior.StateHidden;
                    ViewModel.ApplyFilters();
                    break;
                case Resource.Id.art_dir_feed_clear_filters_button:
                    _filtersBottomSheetBehavior.State = BottomSheetBehavior.StateHidden;
                    ViewModel.ResetFilters();
                    break;
                case Resource.Id.art_dir_feed_filters_button:
                    _filtersBottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;
                    break;
                case Resource.Id.art_dir_feed_filters_close_bs_button:
                    _filtersBottomSheetBehavior.State = BottomSheetBehavior.StateHidden;
                    break;
            }
        }
    }
}
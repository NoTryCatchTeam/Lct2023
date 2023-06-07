using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Android.Animation;
using Android.Content.Res;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.Core.Widget;
using AndroidX.RecyclerView.Widget;
using Com.Google.Android.Exoplayer2;
using Google.Android.Material.Button;
using Google.Android.Material.Card;
using Lct2023.Android.Bindings;
using Lct2023.Android.Decorations;
using Lct2023.Android.Helpers;
using Lct2023.Converters;
using Lct2023.ViewModels.Feed;
using Lct2023.ViewModels.Map;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.ValueConverters;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;
using MvvmCross.DroidX.RecyclerView.Model;
using MvvmCross.Platforms.Android.Binding;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.ViewModels;
using ReactiveUI;
using Square.Picasso;
using static Android.Icu.Text.CaseMap;
using static Android.Util.EventLogTags;
using static Com.Google.Android.Exoplayer2.UI.SubtitleView;

namespace Lct2023.Android.Adapters;

public class FeedListAdapter<TContextViewModel> : BaseTemplatedRecyclerViewAdapter<FeedItemViewModel, FeedListAdapter<TContextViewModel>.BaseFeedItemViewHolder>, INotifyPropertyChanged
    where TContextViewModel: MvxViewModel
{
    private (Func<View> ViewCreator, Action<View, IMvxAndroidBindingContext, MvxFluentBindingDescriptionSet<FeedHeaderItemViewHolder, TContextViewModel>> ViewBinder)[] _headers;
    private TContextViewModel _contextViewModel;

    public FeedListAdapter(IMvxAndroidBindingContext bindingContext, TContextViewModel contextViewModel, params (Func<View> ViewCreator, Action<View, IMvxAndroidBindingContext, MvxFluentBindingDescriptionSet<FeedHeaderItemViewHolder, TContextViewModel>> ViewBinder)[] headers)
        : base(bindingContext)
    {
        _headers = headers;
        _contextViewModel = contextViewModel;

        this.WhenAnyValue(vm => vm.IsLoadingMore)
            .Skip(1)
            .Where(_ => Items != null)
            .Subscribe(_ =>
            {
                switch (IsLoadingMore)
                {
                    case true:
                        NotifyItemInserted(Items.Count);
                        break;
                    default:
                        NotifyItemRemoved(Items.Count);
                        break;
                }
            });
    }

    public ObservableCollection<FeedItemViewModel> Items { get; set; }

    public override int ItemCount => Items.Count switch
    {
        _ when IsLoadingMore => Items.Count + 1,
        _ => Items.Count
    };

    public bool IsLoadingMore { get; set; }

    protected override View InflateViewForHolder(ViewGroup parent, int viewType, IMvxAndroidBindingContext bindingContext)
    => viewType switch
    {
        Resource.Layout.FeedItemView or Resource.Layout.FeedLoadItemView => base.InflateViewForHolder(parent, viewType, bindingContext),

        // header
        _ => _headers.ElementAtOrDefault((viewType + 1) * (-1)).ViewCreator(),
    };

    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
    {
        base.OnBindViewHolder(holder, position);
        if (position < _headers.Length && holder is IMvxRecyclerViewHolder mvxRecyclerViewHolder)
        {
            mvxRecyclerViewHolder.DataContext = _contextViewModel;
        }
    }

    protected override Func<View, int, IMvxAndroidBindingContext, BaseFeedItemViewHolder> BindableTemplatedViewHolderCreator
    => (v, i, c) => i switch
    {
        Resource.Layout.FeedItemView => new FeedItemViewHolder(v, c),
        Resource.Layout.FeedLoadItemView => new FeedLoadingItemViewHolder(v, c),
        _ => new FeedHeaderItemViewHolder(v, c, _headers.ElementAtOrDefault((i + 1) * (-1)).ViewBinder),
    };
    
    public override int GetItemViewType(int position) =>
        position switch
        {
            // header
            _ when position < _headers.Length => ((position * (-1)) - 1),
            _ when Items.Count == position => Resource.Layout.FeedLoadItemView,
            _ => Resource.Layout.FeedItemView,
        };

    public abstract class BaseFeedItemViewHolder : BaseViewHolder
    {
        protected BaseFeedItemViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
        }
    }

    private class FeedItemViewHolder : BaseFeedItemViewHolder
    {
        private readonly ImageView _image;
        private readonly TextView _description;
        private readonly MaterialButton _moreDescriptionButton;

        public FeedItemViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
            var headerImageBackground = itemView.FindViewById<MaterialCardView>(Resource.Id.feed_item_header_image_background);
            var title = itemView.FindViewById<TextView>(Resource.Id.feed_item_title);
            var publishDate = itemView.FindViewById<TextView>(Resource.Id.feed_item_publish_date);
            var actionButton = itemView.FindViewById<MaterialButton>(Resource.Id.feed_item_action_button);
            _description = itemView.FindViewById<TextView>(Resource.Id.feed_item_description);
            _description.Alpha = 0;
            _moreDescriptionButton = itemView.FindViewById<MaterialButton>(Resource.Id.feed_item_more_description_button);
            _image = itemView.FindViewById<ImageView>(Resource.Id.feed_item_image);
            var likeButton = itemView.FindViewById<MaterialButton>(Resource.Id.feed_item_like_button);
            var topArtCategoryButton = itemView.FindViewById<MaterialButton>(Resource.Id.feed_item_top_art_category_button);
            var container = itemView.FindViewById<MaterialCardView>(Resource.Id.feed_item_container);
            
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<FeedItemViewHolder, FeedItemViewModel>();
                
                set.Bind(headerImageBackground)
                    .For(nameof(CardViewBackgroundColorByHexBinding))
                    .To(vm => vm.ArtCategories)
                    .WithConversion(new AnyExpressionConverter<IEnumerable<string>, string>(items => items?.FirstOrDefault() switch
                    {
                        "Музыка" => "#D19EB5",
                        _ => "#A4CF57",
                    }));

                set.Bind(title)
                    .For(v => v.Text)
                    .To(vm => vm.Title);
                
                set.Bind(publishDate)
                    .For(v => v.Text)
                    .To(vm => vm.PublishedAt);
                
                set.Bind(_description)
                    .For(v => v.Text)
                    .To(vm => vm.Description);
                
                set.Bind(_image)
                    .For(v => v.BindVisible())
                    .To(vm => vm.ImageUrl)
                    .WithConversion(new AnyExpressionConverter<string, bool>(url => !string.IsNullOrEmpty(url)));
                
                set.Bind(topArtCategoryButton)
                    .For(v => v.BindVisible())
                    .To(vm => vm.ArtCategories)
                    .WithConversion(new AnyExpressionConverter<IEnumerable<string>, bool>(items => items?.Any() == true));

                set.Bind(topArtCategoryButton)
                    .For(v => v.Text)
                    .To(vm => vm.ArtCategories)
                    .WithConversion(new AnyExpressionConverter<IEnumerable<string>, string>(items => items?.FirstOrDefault()));

                set.Bind(topArtCategoryButton)
                    .For(nameof(ButtonBackgroundByIdBinding))
                    .To(vm => vm.ArtCategories)
                    .WithConversion(new AnyExpressionConverter<IEnumerable<string>, int>(items => items?.FirstOrDefault() switch
                    {
                        "Музыка" => Resource.Color.musicArtDirectionBgColor,
                        "Хореография" => Resource.Color.horeoArtDirectionBgColor,
                        "Рисование" => Resource.Color.drawArtDirectionBgColor,
                        "Театр" or "Цирк" => Resource.Color.theatreArtDirectionBgColor,
                        _ => Resource.Color.defaultArtDirectionBgColor,
                    }));
                
                set.Bind(topArtCategoryButton)
                    .For(nameof(TextViewTextColorBinding))
                    .To(vm => vm.ArtCategories)
                    .WithConversion(new AnyExpressionConverter<IEnumerable<string>, ColorStateList>(items => items?.FirstOrDefault() switch
                    {
                        "Музыка" => itemView.Context.Resources.GetColorStateList(Resource.Color.musicArtDirectionTextColor),
                        "Хореография" => itemView.Context.Resources.GetColorStateList(Resource.Color.horeoArtDirectionTextColor),
                        "Рисование" => itemView.Context.Resources.GetColorStateList(Resource.Color.drawArtDirectionTextColor),
                        "Театр" or "Цирк" => itemView.Context.Resources.GetColorStateList(Resource.Color.theatreArtDirectionTextColor),
                        _ => itemView.Context.Resources.GetColorStateList(Resource.Color.defaultArtDirectionTextColor),
                    }));

                set
                    .Bind(container)
                    .For(v => v.BindClick())
                    .To(vm => vm.ItemClickCommand)
                    .WithConversion<MvxCommandParameterValueConverter>(ViewModel);

                set
                    .Bind(container)
                    .For(v => v.BindLongClick())
                    .To(vm => vm.ItemClickCommand)
                    .WithConversion<MvxCommandParameterValueConverter>(ViewModel);

                set
                    .Bind(_moreDescriptionButton)
                    .For(v => v.BindClick())
                    .To(vm => vm.ExpandCommand)
                    .WithConversion<MvxCommandParameterValueConverter>(ViewModel);


                set.Apply();
            });
        }

        public override void Bind()
        {
            base.Bind();
            
            Picasso.Get()
                .Load(ViewModel.ImageUrl)
            .Into(_image);

            var alphaAnimator = ObjectAnimator.OfFloat(_description, "alpha", 0.0f, 1.0f);
            alphaAnimator.SetDuration(1000);

            _description.Post(() =>
            {
                var height = _description.Height;
                if (height == 0)
                {
                    return;
                }

                height = DimensUtils.PxToDp(_description.Context, height);

                if (height < 150)
                {
                    alphaAnimator.Start();
                    _moreDescriptionButton.Visibility = ViewStates.Gone;
                    return;
                }

                _moreDescriptionButton.Visibility = ViewStates.Visible;

                var set = CreateBindingSet();


                set
                    .Bind(_moreDescriptionButton)
                    .For(nameof(ButtonIconResourceBinding))
                    .To(vm => vm.Expanded)
                    .WithConversion(new AnyExpressionConverter<bool, int>(expanded => expanded ? Resource.Drawable.ic_chevron_bottom : Resource.Drawable.exo_ic_chevron_right));

                set
                    .Bind(_moreDescriptionButton)
                    .For(v => v.Text)
                    .To(vm => vm.Expanded)
                    .WithConversion(new AnyExpressionConverter<bool, string>(expanded => expanded ? "Свернуть" : "Eщё"));


                set.Bind(_description)
                    .For(nameof(TextViewMaxLinesBinding))
                    .To(vm => vm.Expanded)
                    .WithConversion(new AnyExpressionConverter<bool, int>(expanded => expanded ? int.MaxValue : 6));

                set.Apply();

                alphaAnimator.Start();
            });
        }
    }

    private class FeedLoadingItemViewHolder : BaseFeedItemViewHolder
    {
        public FeedLoadingItemViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
        }
    }

    public class FeedHeaderItemViewHolder : BaseFeedItemViewHolder
    {
        private Action<View, IMvxAndroidBindingContext, MvxFluentBindingDescriptionSet<FeedHeaderItemViewHolder, TContextViewModel>> _viewBinder;
        private bool _inited;

        public FeedHeaderItemViewHolder(View itemView, IMvxAndroidBindingContext context, Action<View, IMvxAndroidBindingContext, MvxFluentBindingDescriptionSet<FeedHeaderItemViewHolder, TContextViewModel>> viewBinder)
            : base(itemView, context)
        {
            _viewBinder = viewBinder;
        }

        public override void Bind()
        {
            base.Bind();

            if (_inited)
            {
                return;
            }

            _inited = true;

            var set = this.CreateBindingSet<FeedHeaderItemViewHolder, TContextViewModel>();

            _viewBinder?.Invoke(ItemView, (IMvxAndroidBindingContext)BindingContext, set);

            set.Apply();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}

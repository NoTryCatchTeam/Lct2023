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
using Google.Android.Material.Button;
using Google.Android.Material.Card;
using Lct2023.Android.Bindings;
using Lct2023.Android.Helpers;
using Lct2023.Converters;
using Lct2023.ViewModels.Feed;
using Lct2023.ViewModels.Map;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using ReactiveUI;
using Square.Picasso;
using static Android.Util.EventLogTags;

namespace Lct2023.Android.Adapters;

public class FeedListAdapter : BaseTemplatedRecyclerViewAdapter<FeedItemViewModel, FeedListAdapter.BaseFeedItemViewHolder>, INotifyPropertyChanged
{
    public FeedListAdapter(IMvxAndroidBindingContext bindingContext)
        : base(bindingContext)
    {
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
    
    public override int ItemCount => IsLoadingMore ? Items.Count + 1 : Items.Count;

    public bool IsLoadingMore { get; set; }

    protected override Func<View, int, IMvxAndroidBindingContext, BaseFeedItemViewHolder> BindableTemplatedViewHolderCreator
    => (v, i, c) => i switch
    {
        Resource.Layout.FeedItemView => new FeedItemViewHolder(v, c),
        Resource.Layout.FeedLoadItemView => new FeedLoadingItemViewHolder(v, c),
    };
    
    public override int GetItemViewType(int position) =>
        (Items.Count == position) switch
        {
            false => Resource.Layout.FeedItemView,
            _ => Resource.Layout.FeedLoadItemView,
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
                        _ => Resource.Color.defaultArtDirectionBgColor,
                    }));
                
                set.Bind(topArtCategoryButton)
                    .For(nameof(TextViewTextColorBinding))
                    .To(vm => vm.ArtCategories)
                    .WithConversion(new AnyExpressionConverter<IEnumerable<string>, ColorStateList>(items => items?.FirstOrDefault() switch
                    {
                        "Музыка" => itemView.Context.Resources.GetColorStateList(Resource.Color.musicArtDirectionTextColor),
                        _ => itemView.Context.Resources.GetColorStateList(Resource.Color.defaultArtDirectionTextColor),
                    }));


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
                    return;
                }

                _moreDescriptionButton.Visibility = ViewStates.Visible;

                _moreDescriptionButton.Click += MoreDescriptionButtonClick;

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

        private void MoreDescriptionButtonClick(object sender, EventArgs e) => ViewModel.Expanded = !ViewModel.Expanded;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _moreDescriptionButton.Click -= MoreDescriptionButtonClick;
            }

            base.Dispose(disposing);
        }
    }

    private class FeedLoadingItemViewHolder : BaseFeedItemViewHolder
    {
        public FeedLoadingItemViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}

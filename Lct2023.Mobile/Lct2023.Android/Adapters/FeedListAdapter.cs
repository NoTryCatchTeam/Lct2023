using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Android.Content.Res;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Google.Android.Material.Button;
using Google.Android.Material.Card;
using Lct2023.Android.Bindings;
using Lct2023.Converters;
using Lct2023.ViewModels.Feed;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using ReactiveUI;

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
        public FeedItemViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
            var headerImageBackground = itemView.FindViewById<MaterialCardView>(Resource.Id.feed_item_header_image_background);
            var title = itemView.FindViewById<TextView>(Resource.Id.feed_item_title);
            var publishDate = itemView.FindViewById<TextView>(Resource.Id.feed_item_publish_date);
            var actionButton = itemView.FindViewById<MaterialButton>(Resource.Id.feed_item_action_button);
            var description = itemView.FindViewById<TextView>(Resource.Id.feed_item_description);
            var image = itemView.FindViewById<ImageView>(Resource.Id.feed_item_image);
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
                
                set.Bind(description)
                    .For(v => v.Text)
                    .To(vm => vm.Description);
                
                set.Bind(image)
                    .For(v => v.BindVisible())
                    .To(vm => vm.ImageUrl)
                    .WithConversion(new AnyExpressionConverter<string, bool>(url => !string.IsNullOrEmpty(url)));
                
                set.Bind(image)
                    .For(nameof(ImageViewByUrlBinding))
                    .To(vm => vm.ImageUrl);
                
                set.Bind(topArtCategoryButton)
                    .For(v => v.BindVisible())
                    .To(vm => vm.ArtCategories)
                    .WithConversion(new AnyExpressionConverter<IEnumerable<string>, bool>(items => items?.Any() == true));

                set.Bind(topArtCategoryButton)
                    .For(v => v.Text)
                    .To(vm => vm.ArtCategories)
                    .WithConversion(new AnyExpressionConverter<IEnumerable<string>, string>(items => items?.FirstOrDefault()));

                set.Bind(topArtCategoryButton)
                    .For(nameof(CardViewBackgroundColorByHexBinding))
                    .To(vm => vm.ArtCategories)
                    .WithConversion(new AnyExpressionConverter<IEnumerable<string>, string>(items => items?.FirstOrDefault() switch
                    {
                        "Музыка" => "#d19eb5",
                        _ => "#a3ce55",
                    }));
                
                set.Bind(topArtCategoryButton)
                    .For(nameof(TextViewTextColorBinding))
                    .To(vm => vm.ArtCategories)
                    .WithConversion(new AnyExpressionConverter<IEnumerable<string>, ColorStateList>(items => items?.FirstOrDefault() switch
                    {
                        "Музыка" => itemView.Context.Resources.GetColorStateList(Color.ParseColor("#C07290")),
                        _ => itemView.Context.Resources.GetColorStateList(Color.ParseColor("#8EB04D")),
                    }));

                
                set.Apply();
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

    public event PropertyChangedEventHandler PropertyChanged;
}

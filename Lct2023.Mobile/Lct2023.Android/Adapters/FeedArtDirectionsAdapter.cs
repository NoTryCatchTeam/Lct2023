using System;
using Android.Content.Res;
using Android.Views;
using Android.Widget;
using Google.Android.Material.Card;
using Google.Android.Material.CheckBox;
using Lct2023.Android.Bindings;
using Lct2023.Converters;
using Lct2023.ViewModels.Common;
using Lct2023.ViewModels.Feed;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace Lct2023.Android.Adapters
{
    public class FeedArtDirectionsAdapter : BaseRecyclerViewAdapter<FeedArtDirectionItemViewModel, FeedArtDirectionsAdapter.FeedArtDirectionItemViewHolder>
    {
        public FeedArtDirectionsAdapter(IMvxAndroidBindingContext bindingContext)
            : base(bindingContext)
        {
        }

        protected override Func<View, IMvxAndroidBindingContext, FeedArtDirectionItemViewHolder> BindableViewHolderCreator =>
            (v, c) => new FeedArtDirectionItemViewHolder(v, c);

        public class FeedArtDirectionItemViewHolder : BaseViewHolder
        {
            public FeedArtDirectionItemViewHolder(View itemView, IMvxAndroidBindingContext context)
                : base(itemView, context)
            {
                var title = itemView.FindViewById<TextView>(Resource.Id.feed_art_direction_item_title);
                var image = itemView.FindViewById<ImageView>(Resource.Id.feed_art_direction_item_image);
                var background = itemView.FindViewById<MaterialCardView>(Resource.Id.feed_art_direction_item_image_background);

                this.DelayBind(() =>
                {
                    var set = this.CreateBindingSet<FeedArtDirectionItemViewHolder, FeedArtDirectionItemViewModel>();

                    set.Bind(title)
                        .For(v => v.Text)
                        .To(vm => vm.Title);

                    set.Bind(image)
                        .For(nameof(ImageViewByResIdBinding))
                        .To(vm => vm.Title)
                        .WithConversion(new AnyExpressionConverter<string, int>(name => name switch
                        {
                            "Хореография" => Resource.Drawable.image_horeo_art_dir,
                            "Рисование" => Resource.Drawable.image_draw_art_dir,
                            "Театр" or "Цирк" => Resource.Drawable.image_theatre_art_dir,
                            "Музыка" => Resource.Drawable.image_music_art_dir,
                            _ => Resource.Drawable.image_music_art_dir,
                        }));

                    set.Bind(background)
                        .For(nameof(CardViewBackgroundColorByHexBinding))
                        .To(vm => vm.Title)
                        .WithConversion(new AnyExpressionConverter<string, string>(name => name switch
                        {
                            "Хореография" => "#F6F4BF",
                            "Рисование" => "#AAC5D2",
                            "Театр" or "Цирк" => "#D09EB5",
                            "Музыка" => "#a4cf57",
                            _ => "#a4cf57",
                        }));

                    set.Apply();
                });
            }
        }
    }
}
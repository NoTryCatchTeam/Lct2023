using System;
using Android.Views;
using Android.Widget;
using DataModel.Definitions.Enums;
using Lct2023.Android.Bindings;
using Lct2023.Converters;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace Lct2023.Android.Adapters;

public class ArtDirectionAdapter : BaseRecyclerViewAdapter<ArtDirectionType, ArtDirectionAdapter.ArtDirectionViewHolder>
{
    public ArtDirectionAdapter(IMvxAndroidBindingContext bindingContext)
        : base(bindingContext)
    {
    }

    protected override Func<View, IMvxAndroidBindingContext, ArtDirectionViewHolder> BindableViewHolderCreator =>
        (v, c) => new ArtDirectionViewHolder(v, c);

    public class ArtDirectionViewHolder : BaseViewHolder
    {
        public ArtDirectionViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
            var image = itemView.FindViewById<ImageView>(Resource.Id.art_direction_item_image);
            var text = itemView.FindViewById<TextView>(Resource.Id.art_direction_item_title);

            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<ArtDirectionViewHolder, ArtDirectionType>();

                set.Bind(image)
                    .For(nameof(ImageViewByIdBinding))
                    .To(vm => vm)
                    .WithConversion(new AnyExpressionConverter<ArtDirectionType, int>(value =>
                        value switch
                        {
                            ArtDirectionType.Cello => Resource.Drawable.image_direction_cello,
                            ArtDirectionType.Drums => Resource.Drawable.image_direction_drums,
                            ArtDirectionType.Horn => Resource.Drawable.image_direction_horn,
                        }));
                
                set.Bind(text)
                    .For(v => v.Text)
                    .To(vm => vm)
                    .WithConversion(new AnyExpressionConverter<ArtDirectionType, string>(value =>
                        value switch
                        {
                            ArtDirectionType.Cello => "Виолончель",
                            ArtDirectionType.Drums => "Ударные",
                            ArtDirectionType.Horn => "Валторна",
                        }));

                set.Apply();
            });
        }
    }
}

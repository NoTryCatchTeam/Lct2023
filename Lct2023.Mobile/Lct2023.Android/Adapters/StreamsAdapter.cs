using System;
using Android.Views;
using Android.Widget;
using Lct2023.Converters;
using Lct2023.ViewModels.Art;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.Target;

namespace Lct2023.Android.Adapters;

public class StreamsAdapter : BaseRecyclerViewAdapter<StreamItemViewModel, StreamsAdapter.StreamViewHolder>
{
    public StreamsAdapter(IMvxAndroidBindingContext bindingContext)
        : base(bindingContext)
    {
    }

    protected override Func<View, IMvxAndroidBindingContext, StreamViewHolder> BindableViewHolderCreator =>
        (v, c) => new StreamViewHolder(v, c);

    public class StreamViewHolder : BaseViewHolder
    {
        public StreamViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
            var image = itemView.FindViewById<ImageView>(Resource.Id.art_direction_item_image);
            var text = itemView.FindViewById<TextView>(Resource.Id.art_direction_item_title);

            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<StreamViewHolder, StreamItemViewModel>();

                set.Bind(image)
                    .For(nameof(MvxImageViewResourceNameTargetBinding))
                    .To(vm => vm.Name)
                    .WithConversion(new AnyExpressionConverter<string, int>(value =>
                        value switch
                        {
                            "Скрипка" => Resource.Drawable.image_direction_cello,
                            "Аккордеон" => Resource.Drawable.image_direction_horn,
                            _ => Resource.Drawable.image_direction_drums,
                        }));
                
                set.Bind(text)
                    .For(v => v.Text)
                    .To(vm => vm.Name);

                set.Apply();
            });
        }
    }
}

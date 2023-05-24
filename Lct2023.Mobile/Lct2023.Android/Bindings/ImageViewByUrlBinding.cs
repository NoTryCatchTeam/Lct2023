using Android.Net;
using Android.Widget;
using MvvmCross.Platforms.Android.Binding.Target;
using Square.Picasso;

namespace Lct2023.Android.Bindings;

public class ImageViewByUrlBinding : MvxAndroidTargetBinding<ImageView, string>
{
    public ImageViewByUrlBinding(ImageView target)
        : base(target)
    {
    }

    protected override void SetValueImpl(ImageView target, string value)
    {
        Picasso.Get()
            .Load(Uri.Parse(value))
            .Into(target);
    }
}

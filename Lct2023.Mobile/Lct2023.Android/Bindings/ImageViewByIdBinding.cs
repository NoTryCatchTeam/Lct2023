using Android.Widget;
using MvvmCross.Platforms.Android.Binding.Target;

namespace Lct2023.Android.Bindings;

public class ImageViewByIdBinding: MvxAndroidTargetBinding<ImageView, int>
{
    public ImageViewByIdBinding(ImageView imageView)
        : base(imageView)
    {
    }

    protected override void SetValueImpl(ImageView imageView, int resId) => imageView?.SetImageResource(resId);
}

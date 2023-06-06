using System;
using Android.Widget;
using MvvmCross.Platforms.Android.Binding.Target;

namespace Lct2023.Android.Bindings
{
	public class ImageViewByResIdBinding : MvxAndroidTargetBinding<ImageView, int>
    {
        public ImageViewByResIdBinding(ImageView target)
            : base(target)
        {
        }

        protected override void SetValueImpl(ImageView target, int value) => target.SetImageResource(value);
    }
}


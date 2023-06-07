using System;
using Android.Widget;
using MvvmCross.Platforms.Android.Binding.Target;

namespace Lct2023.Android.Bindings
{
    public class TextViewMaxLinesBinding : MvxAndroidTargetBinding<TextView, int>
    {
        public TextViewMaxLinesBinding(TextView target)
            : base(target)
        {
        }

        protected override void SetValueImpl(TextView target, int value) =>
            target.SetMaxLines(value);
    }
}
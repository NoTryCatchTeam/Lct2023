using Android.Content.Res;
using Android.Widget;
using MvvmCross.Platforms.Android.Binding.Target;

namespace Lct2023.Android.Bindings;

public class TextViewTextColorBinding : MvxAndroidTargetBinding<TextView, ColorStateList>
{
    public TextViewTextColorBinding(TextView target)
        : base(target)
    {
    }

    protected override void SetValueImpl(TextView target, ColorStateList value) =>
        target.SetTextColor(value);
}

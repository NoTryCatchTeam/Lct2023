using Android.Graphics;
using Google.Android.Material.Button;
using MvvmCross.Platforms.Android.Binding.Target;

namespace Lct2023.Android.Bindings;

public class ButtonBackgroundByIdBinding : MvxAndroidTargetBinding<MaterialButton, int>
{
    public ButtonBackgroundByIdBinding(MaterialButton target)
        : base(target)
    {
    }

    protected override void SetValueImpl(MaterialButton target, int value) =>
        target.BackgroundTintList = target.Context.Resources.GetColorStateList(value);
}

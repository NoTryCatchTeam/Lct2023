using Android.Graphics;
using Google.Android.Material.Button;
using MvvmCross.Platforms.Android.Binding.Target;

namespace Lct2023.Android.Bindings;

public class ButtonBackgroundByHexBinding : MvxAndroidTargetBinding<MaterialButton, string>
{
    public ButtonBackgroundByHexBinding(MaterialButton target)
        : base(target)
    {
    }

    protected override void SetValueImpl(MaterialButton target, string value) =>
        target.BackgroundTintList = target.Context.Resources.GetColorStateList(Color.ParseColor(value), null);
}

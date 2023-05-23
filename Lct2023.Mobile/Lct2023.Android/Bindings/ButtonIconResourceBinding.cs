using Google.Android.Material.Button;
using MvvmCross.Platforms.Android.Binding.Target;

namespace Lct2023.Android.Bindings;

public class ButtonIconResourceBinding : MvxAndroidTargetBinding<MaterialButton, int>
{
    public ButtonIconResourceBinding(MaterialButton target)
        : base(target)
    {
    }

    protected override void SetValueImpl(MaterialButton target, int value) =>
        target.SetIconResource(value);
}

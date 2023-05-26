using Google.Android.Material.Card;
using MvvmCross.Platforms.Android.Binding.Target;

namespace Lct2023.Android.Bindings;

public class CardViewStrokeColorBinding : MvxAndroidTargetBinding<MaterialCardView, int>
{
    public CardViewStrokeColorBinding(MaterialCardView target)
        : base(target)
    {
    }

    protected override void SetValueImpl(MaterialCardView target, int value) =>
        target.SetStrokeColor(target.Context.Resources.GetColorStateList(value, null));
}

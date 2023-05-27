using Android.Graphics;
using Google.Android.Material.Card;
using MvvmCross.Platforms.Android.Binding.Target;

namespace Lct2023.Android.Bindings;

public class CardViewBackgroundColorByHexBinding : MvxAndroidTargetBinding<MaterialCardView, string>
{
    public CardViewBackgroundColorByHexBinding(MaterialCardView target)
        : base(target)
    {
    }

    protected override void SetValueImpl(MaterialCardView target, string value) =>
        target.SetCardBackgroundColor(Color.ParseColor(value));
}

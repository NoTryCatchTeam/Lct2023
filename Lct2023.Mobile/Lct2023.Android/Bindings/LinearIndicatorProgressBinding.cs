using Google.Android.Material.ProgressIndicator;
using MvvmCross.Platforms.Android.Binding.Target;

namespace Lct2023.Android.Bindings;

public class LinearIndicatorProgressBinding : MvxAndroidTargetBinding<LinearProgressIndicator, int>
{
    public LinearIndicatorProgressBinding(LinearProgressIndicator target)
        : base(target)
    {
    }

    protected override void SetValueImpl(LinearProgressIndicator target, int value) =>
        target.SetProgress(value, true);
}

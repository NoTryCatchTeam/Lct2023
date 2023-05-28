using System;
using AndroidX.Activity;

namespace Lct2023.Android.Callbacks;

public class DefaultBackPressedCallback : OnBackPressedCallback
{
    private readonly Action<DefaultBackPressedCallback> _onBackPressed;
    
    public DefaultBackPressedCallback(bool enabled, Action<DefaultBackPressedCallback> onBackPressed) 
        : base(true)
    {
        _onBackPressed = onBackPressed;
    }

    public override void HandleOnBackPressed() =>
        _onBackPressed(this);
}
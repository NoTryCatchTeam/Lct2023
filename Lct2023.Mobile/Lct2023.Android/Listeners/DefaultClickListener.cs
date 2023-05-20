using System;
using Android.Views;

namespace Lct2023.Android.Listeners;

public class DefaultClickListener : Java.Lang.Object, View.IOnClickListener
{
    private readonly Action<View> _onClick;

    public DefaultClickListener(Action<View> onClick)
    {
        _onClick = onClick;
    }

    public void OnClick(View v) =>
        _onClick(v);
}

using System;
using Android.Views;
using AndroidX.AppCompat.Widget;

namespace Lct2023.Android.Listeners;

public class DefaultMenuItemClickListener : Java.Lang.Object, Toolbar.IOnMenuItemClickListener
{
    private readonly Func<IMenuItem, bool> _onMenuItemClick;

    public DefaultMenuItemClickListener(Func<IMenuItem, bool> onMenuItemClick)
    {
        _onMenuItemClick = onMenuItemClick;
    }

    public bool OnMenuItemClick(IMenuItem item) =>
        _onMenuItemClick(item);
}
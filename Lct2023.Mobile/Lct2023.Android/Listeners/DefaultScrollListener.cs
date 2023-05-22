using System;
using AndroidX.Core.Widget;

namespace Lct2023.Android.Listeners;

public class DefaultScrollListener : Java.Lang.Object, NestedScrollView.IOnScrollChangeListener
{
    private readonly Action<NestedScrollView, int, int, int, int> _onScrollChange;

    public DefaultScrollListener(Action<NestedScrollView, int, int, int, int> onScrollChange)
    {
        _onScrollChange = onScrollChange;
    }

    public void OnScrollChange(NestedScrollView view, int scrollX, int scrollY, int oldScrollX, int oldScrollY) =>
        _onScrollChange(view, scrollX, scrollY, oldScrollX, oldScrollY);
}

using Android.Content;
using Android.Util;
using Google.Android.Material.AppBar;
using Lct2023.Android.Listeners;
using MvvmCross.Commands;

namespace Lct2023.Android.Views;

public class ExtendedToolbar : MaterialToolbar
{
    private IMvxAsyncCommand _navigationIconClickCommand;

    public ExtendedToolbar(Context context)
        : base(context)
    {
    }

    public ExtendedToolbar(Context context, IAttributeSet attrs)
        : base(context, attrs)
    {
    }

    public IMvxAsyncCommand NavigationIconClickCommand
    {
        get => _navigationIconClickCommand;
        set
        {
            _navigationIconClickCommand = value;

            SetNavigationOnClickListener(value != null ?
                new DefaultClickListener(_ => _navigationIconClickCommand.Execute()) :
                null);
        }
    }
}

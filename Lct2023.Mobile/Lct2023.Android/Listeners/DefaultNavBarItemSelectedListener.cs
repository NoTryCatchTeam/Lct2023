using System;
using Android.Views;
using Google.Android.Material.Navigation;

namespace Lct2023.Android.Listeners;

public class DefaultNavBarItemSelectedListener : Java.Lang.Object, NavigationBarView.IOnItemSelectedListener
{
    private readonly Func<IMenuItem, bool> _onNavigationItemSelected;

    public DefaultNavBarItemSelectedListener(Func<IMenuItem, bool> onNavigationItemSelected)
    {
        _onNavigationItemSelected = onNavigationItemSelected;
    }

    public bool OnNavigationItemSelected(IMenuItem menuItem) =>
        _onNavigationItemSelected(menuItem);
}

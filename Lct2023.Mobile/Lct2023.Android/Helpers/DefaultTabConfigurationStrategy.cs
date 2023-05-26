using System;
using Google.Android.Material.Tabs;

namespace Lct2023.Android.Helpers;

public class DefaultTabConfigurationStrategy : Java.Lang.Object, TabLayoutMediator.ITabConfigurationStrategy
{
    private readonly Action<TabLayout.Tab, int> _onConfigureTab;

    public DefaultTabConfigurationStrategy(Action<TabLayout.Tab, int> onConfigureTab)
    {
        _onConfigureTab = onConfigureTab;
    }

    public void OnConfigureTab(TabLayout.Tab tab, int position) =>
        _onConfigureTab(tab, position);
}

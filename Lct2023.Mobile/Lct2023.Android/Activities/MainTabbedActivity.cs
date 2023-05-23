using System.Collections.Generic;
using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.ViewPager2.Widget;
using Google.Android.Material.BottomNavigation;
using Lct2023.Android.Adapters;
using Lct2023.Android.Listeners;
using Lct2023.ViewModels;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace Lct2023.Android.Activities;

[MvxActivityPresentation]
[Activity(ScreenOrientation = ScreenOrientation.Portrait)]
public class MainTabbedActivity : BaseActivity<MainTabbedViewModel>
{
    private readonly Dictionary<int, int> _tabMenuActionsToFragmentPositionsMap;

    public MainTabbedActivity()
    {
        _tabMenuActionsToFragmentPositionsMap = new Dictionary<int, int>
        {
            { Resource.Id.action_main, 0 },
            { Resource.Id.action_courses, 1 },
            { Resource.Id.action_feed, 2 },
            { Resource.Id.action_tasks, 3 },
            { Resource.Id.action_map, 4 },
        };
    }

    protected override void OnCreate(Bundle bundle)
    {
        base.OnCreate(bundle);
        SetContentView(Resource.Layout.MainTabbedActivity);

        var viewPager = FindViewById<ViewPager2>(Resource.Id.main_view_pager);
        var bottomNavigationView = FindViewById<BottomNavigationView>(Resource.Id.main_view_bottom_navigation);

        viewPager.UserInputEnabled = false;
        viewPager.Adapter = new MainViewPagerAdapter(this, 5);

        bottomNavigationView.SetOnItemSelectedListener(new DefaultNavBarItemSelectedListener(
            item =>
            {
                viewPager.SetCurrentItem(_tabMenuActionsToFragmentPositionsMap[item.ItemId], false);

                return true;
            }));
    }
}

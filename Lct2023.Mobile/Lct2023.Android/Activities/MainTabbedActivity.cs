using System.Collections.Generic;
using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.Transitions;
using AndroidX.ViewPager2.Widget;
using Google.Android.Material.BottomNavigation;
using Lct2023.Android.Adapters;
using Lct2023.Android.Listeners;
using Lct2023.Android.Presenters;
using Lct2023.Android.Views;
using Lct2023.ViewModels;

namespace Lct2023.Android.Activities;

[MvxRootActivityPresentation]
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

        var parent = FindViewById<ConstraintLayout>(Resource.Id.main_view_layout);
        var viewPager = FindViewById<ViewPager2>(Resource.Id.main_view_pager);
        var bottomNavigationView = FindViewById<BottomNavigationView>(Resource.Id.main_view_bottom_navigation);
        var maskView = FindViewById<MaskView>(Resource.Id.main_view_mask);

        viewPager.UserInputEnabled = false;
        viewPager.Adapter = new MainViewPagerAdapter(this, 5);
        viewPager.OffscreenPageLimit = viewPager.Adapter.ItemCount;

        bottomNavigationView.SetOnItemSelectedListener(new DefaultNavBarItemSelectedListener(
            item =>
            {
                viewPager.SetCurrentItem(_tabMenuActionsToFragmentPositionsMap[item.ItemId], false);

                return true;
            }));

        maskView.Clickable = true;

        var maskViews = new View[8];

        var i = 1;
        maskView.SetOnClickListener(new DefaultClickListener(_ =>
        {
            var nextMaskView = maskViews[i];
            var nextMaskRect = new Rect();
            nextMaskView.GetDrawingRect(nextMaskRect);
            parent.OffsetDescendantRectToMyCoords(nextMaskView, nextMaskRect);

            maskView.AnimateToRect(nextMaskRect);

            i = i == 7 ? 0 : i + 1;
        }));

        parent.Post(() =>
        {
            var viewPagerRecyclerViewLayoutManager = ((RecyclerView)viewPager.GetChildAt(0)).GetLayoutManager().FindViewByPosition(0);

            maskViews[0] = FindViewById(Resource.Id.action_main);
            maskViews[1] = FindViewById(Resource.Id.action_courses);
            maskViews[2] = FindViewById(Resource.Id.action_feed);
            maskViews[3] = FindViewById(Resource.Id.action_tasks);
            maskViews[4] = FindViewById(Resource.Id.action_map);
            maskViews[5] = viewPagerRecyclerViewLayoutManager.FindViewById(Resource.Id.toolbar_content);
            maskViews[6] = viewPagerRecyclerViewLayoutManager.FindViewById(Resource.Id.main_stories);
            maskViews[7] = viewPagerRecyclerViewLayoutManager.FindViewById(Resource.Id.main_statistics_frame);

            var firstMaskView = maskViews[0];
            var initMaskRect = new Rect();
            firstMaskView.GetDrawingRect(initMaskRect);
            parent.OffsetDescendantRectToMyCoords(firstMaskView, initMaskRect);

            maskView.SetInitialRect(initMaskRect);
        });
    }
}

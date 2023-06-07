using System;
using AndroidX.Fragment.App;
using AndroidX.ViewPager2.Adapter;
using Lct2023.Android.Fragments.MainTabs;
using Lct2023.ViewModels.Courses;
using Lct2023.ViewModels.Feed;
using Lct2023.ViewModels.Main;
using Lct2023.ViewModels.Map;
using Lct2023.ViewModels.Tasks;
using MvvmCross;

namespace Lct2023.Android.Adapters;

public class MainViewPagerAdapter : FragmentStateAdapter
{
    private readonly Fragment[] _fragmentsArray;

    public MainViewPagerAdapter(FragmentActivity activity, int itemsCount)
        : base(activity)
    {
        ItemCount = itemsCount;
        _fragmentsArray = new Fragment[]
        {
            new MainFragment { ViewModel = Mvx.IoCProvider.Resolve<MainViewModel>() },
            new CoursesFragment { ViewModel = Mvx.IoCProvider.Resolve<CoursesViewModel>() },
            new MainFeedFragment { ViewModel = Mvx.IoCProvider.Resolve<MainFeedViewModel>() },
            new TasksFragment { ViewModel = Mvx.IoCProvider.Resolve<TasksViewModel>() },
            new MapFragment { ViewModel = Mvx.IoCProvider.Resolve<MapViewModel>() },
        };
    }

    public override int ItemCount { get; }

    // TODO Research slow navigation
    public override Fragment CreateFragment(int position) =>
        _fragmentsArray[position];
    // position switch
    // {
    //     0 => _fragmentsArray[position] ?? (_fragmentsArray[position] = new MainFragment { ViewModel = Mvx.IoCProvider.Resolve<MainViewModel>() }),
    //     1 => _fragmentsArray[position] ?? (_fragmentsArray[position] = new CoursesFragment { ViewModel = Mvx.IoCProvider.Resolve<CoursesViewModel>() }),
    //     2 => _fragmentsArray[position] ?? (_fragmentsArray[position] = new FeedFragment { ViewModel = Mvx.IoCProvider.Resolve<FeedViewModel>() }),
    //     3 => _fragmentsArray[position] ?? (_fragmentsArray[position] = new TasksFragment { ViewModel = Mvx.IoCProvider.Resolve<TasksViewModel>() }),
    //     4 => _fragmentsArray[position] ?? (_fragmentsArray[position] = new MapFragment { ViewModel = Mvx.IoCProvider.Resolve<MapViewModel>() }),
    //     _ => throw new NotImplementedException(),
    // };
}

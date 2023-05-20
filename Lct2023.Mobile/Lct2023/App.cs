using Lct2023.ViewModels.Courses;
using Lct2023.ViewModels.Feed;
using Lct2023.ViewModels.Main;
using Lct2023.ViewModels.Map;
using Lct2023.ViewModels.Tests;
using MvvmCross;
using MvvmCross.IoC;
using MvvmCross.ViewModels;

namespace Lct2023;

public class App : MvxApplication
{
    public override void Initialize()
    {
        base.Initialize();

        Mvx.IoCProvider.RegisterType<MainViewModel>();
        Mvx.IoCProvider.RegisterType<CoursesViewModel>();
        Mvx.IoCProvider.RegisterType<FeedViewModel>();
        Mvx.IoCProvider.RegisterType<TestsViewModel>();
        Mvx.IoCProvider.RegisterType<MapViewModel>();

        RegisterCustomAppStart<AppStart>();
    }
}

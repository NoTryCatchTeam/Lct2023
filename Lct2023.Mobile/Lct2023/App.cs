using Lct2023.ViewModels.Courses;
using Lct2023.ViewModels.Feed;
using Lct2023.ViewModels.Main;
using Lct2023.ViewModels.Map;
using Lct2023.ViewModels.Tests;
using MvvmCross;
using MvvmCross.IoC;
using System;
using System.Net.Http;
using System.Reflection;
using Lct2023.Business;
using Lct2023.Business.RestServices.Base;
using Lct2023.Definitions;
using Lct2023.Services;
using Lct2023.Services.Implementation;
using MvvmCross.ViewModels;

namespace Lct2023;

public class App : MvxApplication
{
    public override void Initialize()
    {
        base.Initialize();
        
        CreatableTypes(Assembly.GetAssembly(typeof(BaseRestService)))
            .EndingWith("RestService")
            .AsInterfaces()
            .RegisterAsLazySingleton();
        
        Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IXamarinEssentialsWrapper, XamarinEssentialsWrapper>();
        
        Mvx.IoCProvider.LazyConstructAndRegisterSingleton<UserContext, UserContext>();
        
        Mvx.IoCProvider.LazyConstructAndRegisterSingleton(() => new HttpClient(
            new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
            })
        {
            BaseAddress = new Uri(UrlConstants.BASE_URL),
            Timeout = TimeSpan.FromSeconds(60),
        });

        Mvx.IoCProvider.RegisterType<MainViewModel>();
        Mvx.IoCProvider.RegisterType<CoursesViewModel>();
        Mvx.IoCProvider.RegisterType<FeedViewModel>();
        Mvx.IoCProvider.RegisterType<TestsViewModel>();
        Mvx.IoCProvider.RegisterType<MapViewModel>();

        RegisterCustomAppStart<AppStart>();
    }
}

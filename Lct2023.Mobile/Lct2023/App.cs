using System;
using System.Net.Http;
using System.Reflection;
using Lct2023.Business;
using Lct2023.Business.RestServices.Base;
using Lct2023.Definitions;
using Lct2023.Services;
using Lct2023.Services.Implementation;
using MvvmCross;
using MvvmCross.IoC;
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

        RegisterCustomAppStart<AppStart>();
    }
}

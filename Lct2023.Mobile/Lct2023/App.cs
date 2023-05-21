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
using AutoMapper;
using Lct2023.Business.Clients;
using Lct2023.Business.Helpers;
using Lct2023.Business.RestServices.Base;
using Lct2023.Definitions;
using Lct2023.Definitions.Constants;
using Lct2023.Services;
using Lct2023.Services.Implementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MvvmCross.ViewModels;

namespace Lct2023;

public class App : MvxApplication
{
    private IConfiguration _configuration;

    public override void Initialize()
    {
        base.Initialize();

        _configuration = RegisterConfiguration();

        RegisterBusiness();
        RegisterInternalServices();
        RegisterAndValidateMapper();

        Mvx.IoCProvider.RegisterType<MainViewModel>();
        Mvx.IoCProvider.RegisterType<CoursesViewModel>();
        Mvx.IoCProvider.RegisterType<FeedViewModel>();
        Mvx.IoCProvider.RegisterType<TestsViewModel>();
        Mvx.IoCProvider.RegisterType<MapViewModel>();

        RegisterCustomAppStart<AppStart>();
    }

    private IConfiguration RegisterConfiguration()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonStream(Mvx.IoCProvider.Resolve<IFileProvider>().GetStream("appSettings.json"))
            .Build();

        Mvx.IoCProvider.RegisterSingleton(typeof(IConfiguration), configuration);

        return configuration;
    }

    private void RegisterBusiness()
    {
        CreatableTypes(Assembly.GetAssembly(typeof(BaseRestService)))
            .EndingWith("RestService")
            .AsInterfaces()
            .RegisterAsLazySingleton();

        Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IRequestAuthenticator, BusinessRequestAuthenticator>();
        Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IUserContext, UserContext>();

        Mvx.IoCProvider.LazyConstructAndRegisterSingleton(() =>
        {
            var baseUrl = _configuration.GetValue<string>(ConfigurationConstants.AppSettings.HOST);

            return new ApiClient(
                new LoggingHandler(baseUrl, Mvx.IoCProvider.Resolve<ILogger<LoggingHandler>>())
                {
                    ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
                })
            {
                BaseAddress = new Uri(baseUrl),
                Timeout = TimeSpan.FromSeconds(15),
            };
        });

        Mvx.IoCProvider.LazyConstructAndRegisterSingleton(() =>
        {
            var baseUrl = $"{_configuration.GetValue<string>(ConfigurationConstants.AppSettings.CMS_HOST)}{_configuration.GetValue<string>(ConfigurationConstants.AppSettings.API_PATH)}";

            return new CmsClient(
                new LoggingHandler(baseUrl, Mvx.IoCProvider.Resolve<ILogger<LoggingHandler>>())
                {
                    ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
                })
            {
                BaseAddress = new Uri(baseUrl),
                Timeout = TimeSpan.FromSeconds(15),
            };
        });
    }

    private void RegisterInternalServices()
    {
        Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IXamarinEssentialsWrapper, XamarinEssentialsWrapper>();
        Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IUserService, UserService>();
        Mvx.IoCProvider.LazyConstructAndRegisterSingleton<ISecureStorageService, SecureStorageService>();
    }

    private void RegisterAndValidateMapper()
    {
        Mvx.IoCProvider.LazyConstructAndRegisterSingleton(() =>
            new MapperConfiguration(x => x.AddProfile<AppMapperProfile>())
                .CreateMapper());

        Mvx.IoCProvider.Resolve<IMapper>().ConfigurationProvider.AssertConfigurationIsValid();
    }
}

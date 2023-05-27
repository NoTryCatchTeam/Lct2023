using Lct2023.ViewModels.Courses;
using Lct2023.ViewModels.Feed;
using Lct2023.ViewModels.Main;
using Lct2023.ViewModels.Map;
using MvvmCross;
using MvvmCross.IoC;
using System;
using System.Net.Http;
using System.Reflection;
using AutoMapper;
using FluentValidation;
using Lct2023.Business;
using Lct2023.Business.Helpers;
using Lct2023.Business.RestServices.Base;
using Lct2023.Definitions;
using Lct2023.Definitions.Constants;
using Lct2023.Definitions.Validators;
using Lct2023.Services;
using Lct2023.Services.Implementation;
using Lct2023.ViewModels.Auth;
using Lct2023.ViewModels.Tasks;
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

        RegisterValidators();
        RegisterBusiness();
        RegisterInternalServices();
        RegisterAndValidateMapper();

        Mvx.IoCProvider.RegisterType<MainViewModel>();
        Mvx.IoCProvider.RegisterType<CoursesViewModel>();
        Mvx.IoCProvider.RegisterType<FeedViewModel>();
        Mvx.IoCProvider.RegisterType<TasksViewModel>();
        Mvx.IoCProvider.RegisterType<MapViewModel>();

        RegisterCustomAppStart<AppStart>();
    }

    private IConfiguration RegisterConfiguration()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonStream(Mvx.IoCProvider.Resolve<IPlatformFileProvider>().GetStream("appSettings.json"))
            .AddJsonStream(Mvx.IoCProvider.Resolve<IPlatformFileProvider>().GetStream("secrets.json"))
            .Build();

        Mvx.IoCProvider.RegisterSingleton(typeof(IConfiguration), configuration);

        return configuration;
    }

    private static void RegisterValidators()
    {
        Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IValidator<SignInFields>, SignInFieldsValidator>();
        Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IValidator<SignUpFields>, SignUpFieldsValidator>();
    }

    private void RegisterBusiness()
    {
        BusinessInit.Init(_configuration.GetValue<string>(ConfigurationConstants.AppSettings.API_PATH), _configuration.GetValue<string>(ConfigurationConstants.AppSettings.CMS_PATH));

        CreatableTypes(Assembly.GetAssembly(typeof(BaseRestService)))
            .EndingWith("RestService")
            .AsInterfaces()
            .RegisterAsLazySingleton();

        Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IRequestAuthenticator, BusinessRequestAuthenticator>();
        Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IUserContext, UserContext>();

        Mvx.IoCProvider.LazyConstructAndRegisterSingleton(() =>
        {
            var baseUrl = _configuration.GetValue<string>(ConfigurationConstants.AppSettings.HOST);

            return new HttpClient(
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
        Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IPermissionsService, PermissionsService>();
        Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IMediaService, MediaService>();
    }

    private void RegisterAndValidateMapper()
    {
        Mvx.IoCProvider.LazyConstructAndRegisterSingleton(() =>
            new MapperConfiguration(x => x.AddProfile<AppMapperProfile>())
                .CreateMapper());

        Mvx.IoCProvider.Resolve<IMapper>().ConfigurationProvider.AssertConfigurationIsValid();
    }
}

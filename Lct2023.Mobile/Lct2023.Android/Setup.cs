using Lct2023.Android.Services;
using Lct2023.Factories;
using Lct2023.Services;
using Microsoft.Extensions.Logging;
using MvvmCross;
using MvvmCross.IoC;
using MvvmCross.Platforms.Android.Core;

namespace Lct2023.Android;

public class Setup : MvxAndroidSetup<App>
{
    protected override void InitializeFirstChance(IMvxIoCProvider iocProvider)
    {
        base.InitializeFirstChance(iocProvider);

        Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IDialogService, DialogService>();
        Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IFileProvider, FileProvider>();
    }

    protected override ILoggerProvider CreateLogProvider() =>
        null;

    protected override ILoggerFactory CreateLogFactory() =>
        SharedLoggerFactory.Create();
}

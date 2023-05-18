using Lct2023.Factories;
using Microsoft.Extensions.Logging;
using MvvmCross.Platforms.Android.Core;

namespace Lct2023.Android;

public class Setup : MvxAndroidSetup<App>
{
    protected override ILoggerProvider CreateLogProvider() =>
        null;

    protected override ILoggerFactory CreateLogFactory() =>
        SharedLoggerFactory.Create();
}

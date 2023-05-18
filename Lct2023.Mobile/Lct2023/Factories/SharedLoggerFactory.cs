using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Lct2023.Factories;

public static class SharedLoggerFactory
{
    public static ILoggerFactory Create() =>
        LoggerFactory.Create(builder =>
            builder
                .SetMinimumLevel(LogLevel.Trace)
                .AddSerilog(new LoggerConfiguration()
                    .WriteTo.Console(theme: ConsoleTheme.None, applyThemeToRedirectedOutput: true)
                    .CreateLogger())
        );
}

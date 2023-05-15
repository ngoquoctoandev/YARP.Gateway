using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;
using YARP.Gateway.Options;

namespace YARP.Gateway.Logging;

public static class Extensions
{
    public static WebApplicationBuilder RegisterSerilog(this WebApplicationBuilder builder)
    {
        var appOptions     = builder.Services.BindValidateReturn<AppOptions>(builder.Configuration);
        var serilogOptions = builder.Services.BindValidateReturn<SerilogOptions>(builder.Configuration);
        _ = builder.Host.UseSerilog((_, _, serilogConfig) =>
        {
            if (serilogOptions.EnableErichers) serilogConfig.ConfigureEnrichers(appOptions.Name);
            serilogConfig
                .ConfigureConsoleLogging(serilogOptions.StructuredConsoleLogging)
                .ConfigureSeq(serilogOptions.SeqUrl)
                .SetMinimumLogLevel(serilogOptions.MinimumLogLevel);
            if (serilogOptions.OverideMinimumLogLevel) serilogConfig.OverideMinimumLogLevel();

            PrintAppName(appOptions.Name);
        });

        return builder;
    }

    private static void ConfigureEnrichers(this LoggerConfiguration serilogConfig, string appName)
    {
        serilogConfig
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", appName)
            .Enrich.WithExceptionDetails();
    }

    private static LoggerConfiguration ConfigureSeq(this LoggerConfiguration serilogConfig, string serverUrl) =>
        string.IsNullOrEmpty(serverUrl) ? serilogConfig : serilogConfig.WriteTo.Seq(serverUrl);

    private static LoggerConfiguration ConfigureConsoleLogging(this LoggerConfiguration serilogConfig, bool structuredConsoleLogging) =>
        structuredConsoleLogging
            ? serilogConfig.WriteTo.Async(wt => wt.Console(new CompactJsonFormatter()))
            : serilogConfig.WriteTo.Async(wt => wt.Console());

    private static void SetMinimumLogLevel(this LoggerConfiguration serilogConfig, string minLogLevel)
    {
        var loggingLevelSwitch = new LoggingLevelSwitch
        {
            MinimumLevel = minLogLevel.ToLower() switch
            {
                "debug"       => LogEventLevel.Debug,
                "information" => LogEventLevel.Information,
                "warning"     => LogEventLevel.Warning,
                _             => LogEventLevel.Information
            }
        };
        serilogConfig.MinimumLevel.ControlledBy(loggingLevelSwitch);
    }

    private static void OverideMinimumLogLevel(this LoggerConfiguration serilogConfig)
    {
        serilogConfig
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Yarp", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information);
    }

    private static void PrintAppName(string text)
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine(FiggleFonts.Standard.Render(text));
        Console.ResetColor();
    }
}
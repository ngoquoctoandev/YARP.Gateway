using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;

namespace YARP.Gateway.Logging;

public static class Extensions
{
    public static WebApplicationBuilder RegisterSerilog(this WebApplicationBuilder builder)
    {
        _ = builder.Host.UseSerilog((_, sp, serilogConfig) =>
        {
            var loggerSettings           = sp.GetRequiredService<IOptions<LoggerSettings>>().Value;
            var appName                  = loggerSettings.AppName;
            var seqUrl                   = loggerSettings.SeqUrl;
            var structuredConsoleLogging = loggerSettings.StructuredConsoleLogging;
            var minLogLevel              = loggerSettings.MinimumLogLevel;

            serilogConfig.ConfigureEnrichers(appName)
                .ConfigureConsoleLogging(structuredConsoleLogging)
                .ConfigureSeq(seqUrl)
                .SetMinimumLogLevel(minLogLevel)
                .OverideMinimumLogLevel();

            PrintAppName(appName);
        });

        return builder;
    }

    private static LoggerConfiguration ConfigureEnrichers(this LoggerConfiguration serilogConfig, string appName) =>
        serilogConfig
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", appName)
            .Enrich.WithExceptionDetails()
            .Enrich.FromLogContext();

    private static LoggerConfiguration ConfigureSeq(this LoggerConfiguration serilogConfig, string serverUrl) =>
        string.IsNullOrEmpty(serverUrl) ? serilogConfig : serilogConfig.WriteTo.Seq(serverUrl);

    private static LoggerConfiguration ConfigureConsoleLogging(this LoggerConfiguration serilogConfig, bool structuredConsoleLogging) =>
        structuredConsoleLogging
            ? serilogConfig.WriteTo.Async(wt => wt.Console(new CompactJsonFormatter()))
            : serilogConfig.WriteTo.Async(wt => wt.Console());

    private static void OverideMinimumLogLevel(this LoggerConfiguration serilogConfig)
    {
        serilogConfig
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Yarp", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information);
    }

    private static LoggerConfiguration SetMinimumLogLevel(this LoggerConfiguration serilogConfig, string minLogLevel) =>
        minLogLevel.ToLower() switch
        {
            "debug"       => serilogConfig.MinimumLevel.Debug(),
            "information" => serilogConfig.MinimumLevel.Information(),
            "warning"     => serilogConfig.MinimumLevel.Warning(),
            _             => serilogConfig.MinimumLevel.Information()
        };

    private static void PrintAppName(string text)
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine(FiggleFonts.Standard.Render(text));
        Console.ResetColor();
    }
}
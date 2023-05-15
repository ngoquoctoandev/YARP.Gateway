namespace YARP.Gateway.Configurations;

internal static class Startup
{
    private const string ConfigurationsDirectory = "Configurations";

    internal static WebApplicationBuilder AddConfigurations(this WebApplicationBuilder builder)
    {
        var env = builder.Environment;
        builder.Configuration
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
            .AddJsonFile($"{ConfigurationsDirectory}/logger.json", false, true)
            .AddJsonFile($"{ConfigurationsDirectory}/logger.{env.EnvironmentName}.json", true, true)
            .AddJsonFile($"{ConfigurationsDirectory}/reverseproxy.json", false, true)
            .AddJsonFile($"{ConfigurationsDirectory}/reverseproxy.{env.EnvironmentName}.json", true, true)
            .AddJsonFile($"{ConfigurationsDirectory}/kestrel.json", false, true)
            .AddJsonFile($"{ConfigurationsDirectory}/kestrel.{env.EnvironmentName}.json", true, true)
            .AddJsonFile($"{ConfigurationsDirectory}/cors.json", false, true)
            .AddJsonFile($"{ConfigurationsDirectory}/cors.{env.EnvironmentName}.json", true, true)
            .AddJsonFile($"{ConfigurationsDirectory}/ratelimiting.json", false, true)
            .AddJsonFile($"{ConfigurationsDirectory}/ratelimiting.{env.EnvironmentName}.json", true, true)
            .AddEnvironmentVariables();

        builder.Services.AddOptions<LoggerSettings>().BindConfiguration(nameof(LoggerSettings))
            .Services.AddOptions<KestrelSettings>().BindConfiguration(nameof(KestrelSettings))
            .Services.AddOptions<CorsSettings>().BindConfiguration(nameof(CorsSettings))
            .Services.AddOptions<RateLimitSettings>().BindConfiguration(nameof(RateLimitSettings))
            .ValidateDataAnnotations().ValidateOnStart();

        return builder;
    }
}
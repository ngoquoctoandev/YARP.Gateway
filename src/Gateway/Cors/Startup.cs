using YARP.Gateway.Options;

namespace YARP.Gateway.Cors;

internal static class Startup
{
    private const string CorsPolicy = nameof(CorsPolicy);

    internal static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        var          corsSettings = services.BindValidateReturn<CorsSettings>(configuration);
        List<string> origins      = new();
        if (corsSettings.Angular is not null)
            origins.AddRange(corsSettings.Angular.Split(';', StringSplitOptions.RemoveEmptyEntries));
        if (corsSettings.Blazor is not null)
            origins.AddRange(corsSettings.Blazor.Split(';', StringSplitOptions.RemoveEmptyEntries));
        if (corsSettings.React is not null)
            origins.AddRange(corsSettings.React.Split(';', StringSplitOptions.RemoveEmptyEntries));

        return services.AddCors(opt =>
            opt.AddPolicy(CorsPolicy, policy =>
                policy.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithOrigins(origins.ToArray())));
    }

    internal static IApplicationBuilder UseCorsPolicy(this IApplicationBuilder app) => app.UseCors(CorsPolicy);
}
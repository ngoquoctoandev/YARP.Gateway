using YARP.Gateway;

StaticLogger.EnsureInitialized();
Log.Information("Server Booting Up...");
Log.Information("For documentations and guides, visit https://ngoquoctoan.dev");
Log.Information("To Sponsor this project, visit https://www.buymeacoffee.com/fullstackhero");
try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.AddConfigurations().RegisterSerilog().ConfigureWebHost();
    builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
    builder.Services.AddRateLimiting().AddCorsPolicy();
    builder.Services.AddHealthChecks().AddCheck<ReverseProxyHealthCheck>("ReverseProxyHealthCheck");

    var app = builder.Build();

    app.MapGet("/", async context =>
    {
        var body = new
        {
            ConnectionId = context.Connection.Id,
            IP           = context.GetIpAddress(),
            Timestamp    = DateTimeOffset.Now,
            Message      = "YARP reverse proxy. Made by ngoquoctoandev with 💜"
        };
        await context.Response.WriteAsJsonAsync(body);
    }).RequireRateLimiting(RateLimitPolicy.Sliding);

    app.UseRateLimiter().UseCorsPolicy();
    app.MapHealthChecks("/health");
    app.MapReverseProxy();

    app.Run();
}
catch (Exception ex) when (!ex.GetType().Name.Equals("StopTheHostException", StringComparison.Ordinal))
{
    const string message = "Unhandled exception. Provide the ErrorId {ErrorId} to the support team for further analysis.";
    Log.Fatal(ex, message, Guid.NewGuid());
}
finally
{
    Log.Information("Server Shutting down...");
    Log.CloseAndFlush();
}
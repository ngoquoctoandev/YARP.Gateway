﻿using YARP.Gateway;
using YARP.Gateway.Auth;

StaticLogger.EnsureInitialized();
Log.Information("Server Booting Up...");
Log.Information("For documentations and guides, visit https://ngoquoctoan.dev");
Log.Information("To Sponsor this project, visit https://www.buymeacoffee.com/fullstackhero");
try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.AddConfigurations().RegisterSerilog().ConfigureWebHost();
    builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
    builder.Services.AddRateLimiting(builder.Configuration).AddCorsPolicy(builder.Configuration);
    builder.Services.AddHealthChecks().AddCheck<ReverseProxyHealthCheck>(nameof(ReverseProxyHealthCheck));
    builder.Services.AddAuth(builder.Configuration);

    var app = builder.Build();

    app.UseEndpoints();
    app.UseSerilogRequestLogging()
        .UseRouting()
        .UseAuth()
        .UseRateLimiter()
        .UseCorsPolicy();
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
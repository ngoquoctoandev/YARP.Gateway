namespace YARP.Gateway;

public static class Endpoints
{
    public static IEndpointConventionBuilder UseEndpoints(this IEndpointRouteBuilder app) => app.MapGet("/", async context =>
    {
        var body = new
        {
            ConnectionId = context.Connection.Id,
            IP           = context.GetIpAddress(),
            UserAgent    = context.Request.Headers["User-Agent"].FirstOrDefault(),
            Timestamp    = DateTimeOffset.Now,
            Message      = "YARP reverse proxy. Made by ngoquoctoandev with 💜"
        };
        await context.Response.WriteAsJsonAsync(body);
    }).RequireRateLimiting(RateLimitPolicy.Sliding);
}
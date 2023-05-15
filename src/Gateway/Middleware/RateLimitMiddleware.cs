namespace YARP.Gateway.Middleware;

public static class RateLimitMiddleware
{
    internal static IServiceCollection AddRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            var limiterSettings = services.BuildServiceProvider().GetRequiredService<IOptions<RateLimitSettings>>().Value;

            const string message        = "The %s user policy must be configured in the appsettings.json file.";
            var          anonymousUser  = limiterSettings.UserPolicy.SingleOrDefault(x => x.Name == RateLimitPolicy.Anonymous)  ?? throw new Exception(message.Replace("%s", RateLimitPolicy.Anonymous));
            var          registeredUser = limiterSettings.UserPolicy.SingleOrDefault(x => x.Name == RateLimitPolicy.Registered) ?? throw new Exception(message.Replace("%s", RateLimitPolicy.Registered));

            options.RejectionStatusCode = limiterSettings.QuotaExceededResponse.StatusCode;
            var globalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "N/A";

                return RateLimitPartition.GetConcurrencyLimiter(clientIp, _ => new ConcurrencyLimiterOptions
                {
                    PermitLimit = limiterSettings.GlobalLimiter.PermitLimit,
                    QueueLimit  = limiterSettings.GlobalLimiter.QueueLimit
                });
            });

            options.GlobalLimiter = PartitionedRateLimiter.CreateChained(globalLimiter);

            options.AddPolicy(RateLimitPolicy.Sliding, httpContext =>
            {
                var identityName = httpContext.User.Identity?.Name ?? RateLimitPolicy.Anonymous;
                var isAdmin      = httpContext.User.IsInRole("Admin");

                return identityName switch
                {
                    _ when isAdmin => RateLimitPartition.GetNoLimiter(RateLimitPolicy.Unlimited),
                    RateLimitPolicy.Anonymous => RateLimitPartition.GetFixedWindowLimiter(httpContext.GetIpAddress(), _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = anonymousUser.PermitLimit,
                        QueueLimit  = anonymousUser.QueueLimit,
                        Window      = TimeSpan.FromSeconds(anonymousUser.Window)
                        //SegmentsPerWindow = anonymousUser.SegmentsPerWindow
                    }),
                    _ => RateLimitPartition.GetSlidingWindowLimiter(identityName, _ => new SlidingWindowRateLimiterOptions
                    {
                        PermitLimit       = registeredUser.PermitLimit,
                        QueueLimit        = registeredUser.QueueLimit,
                        Window            = TimeSpan.FromSeconds(registeredUser.Window),
                        SegmentsPerWindow = registeredUser.SegmentsPerWindow
                    })
                };
            });

            options.OnRejected = async (context, cancellationToken) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter = new StringValues(((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo));
                    limiterSettings.QuotaExceededResponse.Message   = limiterSettings.QuotaExceededResponse.MessageRetryAfter.Replace("%s", retryAfter.TotalMinutes.ToString(NumberFormatInfo.InvariantInfo));
                }

                var result = new
                {
                    ErrorCode = limiterSettings.QuotaExceededResponse.StatusCode,
                    limiterSettings.QuotaExceededResponse.Message,
                    Timestamp = DateTimeOffset.Now,
                    User      = context.HttpContext.User.Identity?.Name ?? RateLimitPolicy.Anonymous,
                    ClientIP  = context.HttpContext.GetIpAddress()
                };
                await context.HttpContext.Response.WriteAsJsonAsync(result, cancellationToken);
            };
        });

        return services;
    }
}
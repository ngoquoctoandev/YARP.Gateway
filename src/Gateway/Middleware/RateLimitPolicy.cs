namespace YARP.Gateway.Middleware;

public static class RateLimitPolicy
{
    public const string Anonymous  = nameof(Anonymous);
    public const string Registered = nameof(Registered);
    public const string Unlimited  = nameof(Unlimited);
    public const string Fixed      = nameof(Fixed);
    public const string Sliding    = nameof(Sliding);
}
namespace YARP.Gateway.Middleware;

public class RateLimitSettings
{
    [Required] public bool                  Enabled               { get; set; }
    public            QuotaExceededResponse QuotaExceededResponse { get; set; } = new();
    [Required] public LimiterOptions        GlobalLimiter         { get; set; } = new();
    public            List<LimiterOptions>  UserPolicy            { get; set; } = null!;
}

public class QuotaExceededResponse
{
    [Required]        public string Message           { get; set; } = null!;
    [Required]        public string MessageRetryAfter { get; set; } = null!;
    [Range(100, 511)] public int    StatusCode        { get; set; }
}

public class LimiterOptions
{
    [Required] public string Name              { get; set; } = null!;
    [Required] public int    PermitLimit       { get; set; }
    public            int    Window            { get; set; }
    public            int    QueueLimit        { get; set; }
    public            int    SegmentsPerWindow { get; set; }
    public            bool   AutoReplenishment { get; set; }
}
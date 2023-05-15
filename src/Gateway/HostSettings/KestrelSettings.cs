using YARP.Gateway.Options;

namespace YARP.Gateway.HostSettings;

public class KestrelSettings : IOptionsRoot
{
    [Range(0, 65535)] public int               HttpsPort          { get; init; } = 443;
    [Range(0, 65535)] public int               HttpPort           { get; init; } = 80;
    [Range(0, 100)]   public int               MaxRequestBodySize { get; init; } = 10;
    public                   string            Protocols          { get; init; } = null!;
    public                   List<Certificate> Certificates       { get; init; } = null!;
}

public record Certificate
{
    [Required] public string  Domain   { get; init; } = null!;
    [Required] public string  Path     { get; init; } = null!;
    public            string? Password { get; init; }
}
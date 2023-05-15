using YARP.Gateway.Options;

namespace YARP.Gateway.Cors;

public class CorsSettings : IOptionsRoot
{
    public string? Angular { get; set; }
    public string? Blazor  { get; set; }
    public string? React   { get; set; }
}
using YARP.Gateway.Options;

namespace YARP.Gateway.Auth;

public class JwtSettings : IOptionsRoot
{
    [Required(AllowEmptyStrings = false)] public string Key { get; set; } = null!;
}
namespace YARP.Gateway.Options;

public class AppOptions : IOptionsRoot
{
    [Required(AllowEmptyStrings = false)] public string Name { get; set; } = default!;
}
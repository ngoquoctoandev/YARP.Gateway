using YARP.Gateway.Options;

namespace YARP.Gateway.Logging;

public class SerilogOptions : IOptionsRoot
{
    public string SeqUrl                   { get; set; } = string.Empty;
    public bool   StructuredConsoleLogging { get; set; } = false;
    public bool   EnableErichers           { get; set; } = true;
    public bool   OverideMinimumLogLevel   { get; set; } = false;
    public string MinimumLogLevel          { get; set; } = "Information";
}
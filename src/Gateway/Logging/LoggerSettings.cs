using YARP.Gateway.Options;

namespace YARP.Gateway.Logging;

public class LoggerSettings : IOptionsRoot
{
    public string AppName                  { get; set; } = "YARP.Gateway";
    public string SeqUrl                   { get; set; } = string.Empty;
    public bool   StructuredConsoleLogging { get; set; } = false;
    public string MinimumLogLevel          { get; set; } = "Information";
}
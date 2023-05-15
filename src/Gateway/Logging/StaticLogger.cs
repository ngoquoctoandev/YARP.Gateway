using Serilog.Exceptions;

namespace YARP.Gateway.Logging;

public static class StaticLogger
{
    public static void EnsureInitialized()
    {
        if (Log.Logger is not Logger)
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .WriteTo.Console()
                .WriteTo.Seq("http://localhost:5341/")
                .CreateLogger();
    }
}
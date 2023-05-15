namespace YARP.Gateway;

public static class Helper
{
    public static string GetIpAddress(this HttpContext context) => context.Request.Headers.TryGetValue("X-Forwarded-For", out var header)
        ? header.ToString()
        : context.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "N/A";
}
namespace YARP.Gateway.HostSettings;

public static class WebHostSettings
{
    public static WebApplicationBuilder ConfigureWebHost(this WebApplicationBuilder builder)
    {
        var kestrelSettings = builder.Services.BuildServiceProvider().GetRequiredService<IOptions<KestrelSettings>>().Value;

        Dictionary<string, X509Certificate2> certs = new();
        foreach (var cert in kestrelSettings.Certificates)
        {
            if (string.IsNullOrWhiteSpace(cert.Domain) || string.IsNullOrWhiteSpace(cert.Path)) throw new InvalidOperationException("Invalid certificate configuration. Domain and Path are required.");

            var certificate = new X509Certificate2(cert.Path, cert.Password);
            certs.Add(cert.Domain, certificate);
        }

        builder.WebHost.ConfigureKestrel(serverOptions =>
        {
            serverOptions.AddServerHeader = false;
            serverOptions.ListenAnyIP(kestrelSettings.HttpsPort, listenOptions =>
            {
                listenOptions.KestrelServerOptions.Limits.MaxRequestBodySize = kestrelSettings.MaxRequestBodySize * 1024 * 1024; // sizex1MB
                listenOptions.Protocols                                      = kestrelSettings.Protocols == "Http1AndHttp2AndHttp3" ? HttpProtocols.Http1AndHttp2AndHttp3 : HttpProtocols.Http1AndHttp2;
                listenOptions.UseHttps(httpsOptions =>
                {
                    httpsOptions.SslProtocols               = SslProtocols.Tls13;
                    httpsOptions.CheckCertificateRevocation = true;
                    httpsOptions.ServerCertificateSelector  = (_, domain) => domain != null && certs.TryGetValue(domain, out var cert) ? cert : null;
                });
            });
            serverOptions.ListenAnyIP(kestrelSettings.HttpPort, listenOptions =>
            {
                listenOptions.KestrelServerOptions.Limits.MaxRequestBodySize = kestrelSettings.MaxRequestBodySize * 1024 * 1024; // sizex1MB
                listenOptions.Protocols                                      = kestrelSettings.Protocols == "Http1AndHttp2AndHttp3" ? HttpProtocols.Http1AndHttp2AndHttp3 : HttpProtocols.Http1AndHttp2;
            });
        });

        return builder;
    }
}
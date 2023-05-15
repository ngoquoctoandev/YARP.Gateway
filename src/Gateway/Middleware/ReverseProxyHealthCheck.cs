namespace YARP.Gateway.Middleware;

public class ReverseProxyHealthCheck : IHealthCheck
{
    #region Implementation of IHealthCheck

    /// <summary>
    ///     Runs the health check, returning the status of the component being checked.
    /// </summary>
    /// <param name="context">A context object associated with the current execution.</param>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> that can be used to cancel the health check.</param>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that completes when the health check has finished, yielding the status of the component being checked.</returns>
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new()) => Task.FromResult(HealthCheckResult.Healthy("Reverse Proxy is healthy."));

    #endregion
}
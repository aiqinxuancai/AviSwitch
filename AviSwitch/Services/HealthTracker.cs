using AviSwitch.Config;
using AviSwitch.Runtime;

namespace AviSwitch.Services;

public sealed class HealthTracker
{
    private readonly HealthConfig _config;

    public HealthTracker(AppConfig config)
    {
        _config = config.Health;
    }

    public bool IsHealthy(PlatformState platform, DateTimeOffset now)
    {
        return platform.IsHealthy(now);
    }

    public void ReportSuccess(PlatformState platform)
    {
        platform.ReportSuccess();
    }

    public CircuitBreakResult? ReportFailure(PlatformState platform, int failureThreshold, DateTimeOffset now)
    {
        var threshold = Math.Max(1, failureThreshold);
        var cooldown = TimeSpan.FromSeconds(_config.CooldownSeconds);
        return platform.ReportFailure(threshold, cooldown, now);
    }

    public bool IsRetryableStatusCode(int statusCode)
    {
        return statusCode >= 400;
    }
}

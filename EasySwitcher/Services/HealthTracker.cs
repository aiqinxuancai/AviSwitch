using EasySwitcher.Config;
using EasySwitcher.Runtime;

namespace EasySwitcher.Services;

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

    public void ReportFailure(PlatformState platform, DateTimeOffset now)
    {
        var cooldown = TimeSpan.FromSeconds(_config.CooldownSeconds);
        platform.ReportFailure(_config.FailureThreshold, cooldown, now);
    }

    public bool IsRetryableStatusCode(int statusCode)
    {
        if (_config.RetryOn429 && statusCode == 429)
        {
            return true;
        }

        return statusCode >= _config.RetryableStatusMin && statusCode <= _config.RetryableStatusMax;
    }
}

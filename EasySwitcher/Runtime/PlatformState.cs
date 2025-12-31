using EasySwitcher.Config;

namespace EasySwitcher.Runtime;

public sealed class PlatformState
{
    private readonly object _lock = new();

    public PlatformState(PlatformConfig config, Uri baseUri)
    {
        Config = config;
        BaseUri = baseUri;
    }

    public PlatformConfig Config { get; }
    public Uri BaseUri { get; }

    public int FailureCount { get; private set; }
    public DateTimeOffset? UnhealthyUntil { get; private set; }

    public void ReportSuccess()
    {
        lock (_lock)
        {
            FailureCount = 0;
            UnhealthyUntil = null;
        }
    }

    public void ReportFailure(int threshold, TimeSpan cooldown, DateTimeOffset now)
    {
        lock (_lock)
        {
            FailureCount++;
            if (FailureCount >= threshold)
            {
                UnhealthyUntil = now.Add(cooldown);
            }
        }
    }

    public bool IsHealthy(DateTimeOffset now)
    {
        lock (_lock)
        {
            return UnhealthyUntil is null || UnhealthyUntil <= now;
        }
    }
}

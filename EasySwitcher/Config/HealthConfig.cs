namespace EasySwitcher.Config;

public sealed class HealthConfig
{
    public int FailureThreshold { get; set; } = 2;
    public int CooldownSeconds { get; set; } = 30;
    public int RetryableStatusMin { get; set; } = 500;
    public int RetryableStatusMax { get; set; } = 599;
    public bool RetryOn429 { get; set; } = true;
}

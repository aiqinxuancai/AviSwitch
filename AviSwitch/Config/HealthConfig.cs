namespace AviSwitch.Config;

public sealed class HealthConfig
{
    /// <summary>
    /// 不健康冷却时间（秒，连续熔断按倍数增加）。
    /// </summary>
    public int CooldownSeconds { get; set; } = 60;
}

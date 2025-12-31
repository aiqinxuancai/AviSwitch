namespace EasySwitcher.Config;

public sealed class PlatformConfig
{
    public string Name { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string Group { get; set; } = string.Empty;
    public int Weight { get; set; } = 1;
    public int Priority { get; set; } = 0;
    public string? KeyHeader { get; set; }
    public string? KeyPrefix { get; set; }
    public bool Enabled { get; set; } = true;
}

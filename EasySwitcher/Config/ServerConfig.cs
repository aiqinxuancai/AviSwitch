namespace EasySwitcher.Config;

public sealed class ServerConfig
{
    public string Listen { get; set; } = "http://0.0.0.0:8080";
    public string AuthKey { get; set; } = string.Empty;
    public string DefaultGroup { get; set; } = "default";
    public string Strategy { get; set; } = "round_robin";
    public int TimeoutSeconds { get; set; } = 120;
    public int MaxFailover { get; set; } = 2;
    public int MaxRequestBodyBytes { get; set; } = 10 * 1024 * 1024;
}

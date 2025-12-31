namespace EasySwitcher.Config;

public sealed class GroupConfig
{
    public string? Strategy { get; set; }
    public int? MaxFailover { get; set; }
    public int? TimeoutSeconds { get; set; }
}

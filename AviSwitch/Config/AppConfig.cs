using Tomlyn;
namespace AviSwitch.Config;

[TomlModel]
public sealed class AppConfig
{
    public ServerConfig? Server { get; set; }

    public HealthConfig? Health { get; set; }

    public Dictionary<string, GroupConfig> Groups { get; set; } = new();

    public List<PlatformConfig> Platforms { get; set; } = new();

    public void ApplyDefaultsAndValidate()
    {
        Server ??= new ServerConfig();
        Health ??= new HealthConfig();
        Platforms ??= new List<PlatformConfig>();
        Groups ??= new Dictionary<string, GroupConfig>(StringComparer.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(Server.Listen))
        {
            Server.Listen = "http://0.0.0.0:7085";
        }

        if (string.IsNullOrWhiteSpace(Server.AuthKey))
        {
            throw new InvalidOperationException("server.auth_key is required.");
        }

        if (string.IsNullOrWhiteSpace(Server.DefaultGroup))
        {
            Server.DefaultGroup = "default";
        }

        if (string.IsNullOrWhiteSpace(Server.Strategy))
        {
            Server.Strategy = "weighted";
        }

        if (Server.TimeoutSeconds <= 0)
        {
            Server.TimeoutSeconds = 600;
        }

        if (Server.MaxFailover <= 0)
        {
            Server.MaxFailover = 1;
        }

        if (Server.MaxRequestBodyBytes <= 0)
        {
            Server.MaxRequestBodyBytes = 100 * 1024 * 1024;
        }

        if (Health.CooldownSeconds <= 0)
        {
            Health.CooldownSeconds = 30;
        }

        if (Platforms.Count == 0)
        {
            throw new InvalidOperationException("At least one platform is required.");
        }

        var platformIndex = 1;
        foreach (var platform in Platforms)
        {
            if (string.IsNullOrWhiteSpace(platform.Name))
            {
                platform.Name = $"platform-{platformIndex}";
            }

            if (string.IsNullOrWhiteSpace(platform.BaseUrl))
            {
                throw new InvalidOperationException($"platforms[{platformIndex}].base_url is required.");
            }

            if (string.IsNullOrWhiteSpace(platform.Group))
            {
                platform.Group = Server.DefaultGroup;
            }

            if (platform.Weight <= 0)
            {
                platform.Weight = 1;
            }

            var hasKeyType = !string.IsNullOrWhiteSpace(platform.KeyType);
            var hasApiKey = !string.IsNullOrWhiteSpace(platform.ApiKey);

            if (hasKeyType)
            {
                var normalizedKeyType = platform.KeyType.Trim().ToLowerInvariant();
                if (normalizedKeyType is not ("openai" or "claude" or "gemini"))
                {
                    throw new InvalidOperationException($"platforms[{platformIndex}].key_type must be one of: openai, claude, gemini.");
                }

                platform.KeyType = normalizedKeyType;
            }

            

            platformIndex++;
        }

        var normalizedGroups = new Dictionary<string, GroupConfig>(StringComparer.OrdinalIgnoreCase);
        foreach (var (key, group) in Groups)
        {
            var normalized = string.IsNullOrWhiteSpace(key) ? Server.DefaultGroup : key;
            normalizedGroups[normalized] = group ?? new GroupConfig();
        }

        Groups = normalizedGroups;
    }
}


public sealed class ServerConfig
{
    public string Listen { get; set; } = string.Empty;

    public string AuthKey { get; set; } = string.Empty;

    public string DefaultGroup { get; set; } = string.Empty;

    public string Strategy { get; set; } = string.Empty;

    public int TimeoutSeconds { get; set; }

    public int MaxFailover { get; set; }

    public int MaxRequestBodyBytes { get; set; }

    public bool DebugLog { get; set; }
    
}

public sealed class HealthConfig
{
    public int CooldownSeconds { get; set; }
}

public sealed class GroupConfig
{
    public string Strategy { get; set; } = string.Empty;

    public int MaxFailover { get; set; }

    public int TimeoutSeconds { get; set; }
}

public sealed class PlatformConfig
{
    public string Name { get; set; } = string.Empty;

    public string BaseUrl { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;

    public string Group { get; set; } = string.Empty;

    public int Weight { get; set; }

    public int Priority { get; set; }

    public string KeyType { get; set; } = string.Empty;

    public bool Enabled { get; set; }

    public string KeyHeader { get; set; } = string.Empty;

    public string KeyPrefix { get; set; } = string.Empty;
    

}

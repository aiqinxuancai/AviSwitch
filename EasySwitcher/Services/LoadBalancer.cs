using System.Collections.Concurrent;
using EasySwitcher.Config;
using EasySwitcher.Runtime;

namespace EasySwitcher.Services;

public sealed class LoadBalancer
{
    private readonly PlatformRegistry _registry;
    private readonly HealthTracker _health;
    private readonly ConcurrentDictionary<string, int> _roundRobinIndex = new(StringComparer.OrdinalIgnoreCase);

    public LoadBalancer(PlatformRegistry registry, HealthTracker health)
    {
        _registry = registry;
        _health = health;
    }

    public IReadOnlyList<PlatformState> GetCandidates(string group, GroupConfig? groupConfig, AppConfig config, DateTimeOffset now)
    {
        var platforms = _registry.GetGroup(group).Where(p => p.Config.Enabled).ToList();
        if (platforms.Count == 0)
        {
            return Array.Empty<PlatformState>();
        }

        var healthy = platforms.Where(p => _health.IsHealthy(p, now)).ToList();
        if (healthy.Count == 0)
        {
            healthy = platforms;
        }

        var strategy = (groupConfig?.Strategy ?? config.Server.Strategy).Trim().ToLowerInvariant();
        return strategy switch
        {
            "random" => Shuffle(healthy),
            "weighted" => WeightedOrder(healthy),
            "failover" => OrderByPriority(healthy),
            _ => RoundRobin(healthy, group),
        };
    }

    private IReadOnlyList<PlatformState> RoundRobin(List<PlatformState> platforms, string group)
    {
        if (platforms.Count == 1)
        {
            return platforms;
        }

        var current = _roundRobinIndex.AddOrUpdate(group, 0, (_, value) => unchecked(value + 1));
        var index = (int)((uint)current % (uint)platforms.Count);

        var ordered = new List<PlatformState>(platforms.Count);
        ordered.Add(platforms[index]);
        for (var i = 1; i < platforms.Count; i++)
        {
            ordered.Add(platforms[(index + i) % platforms.Count]);
        }
        return ordered;
    }

    private static IReadOnlyList<PlatformState> Shuffle(List<PlatformState> platforms)
    {
        var list = new List<PlatformState>(platforms);
        for (var i = list.Count - 1; i > 0; i--)
        {
            var j = Random.Shared.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
        return list;
    }

    private static IReadOnlyList<PlatformState> WeightedOrder(List<PlatformState> platforms)
    {
        if (platforms.Count == 1)
        {
            return platforms;
        }

        var totalWeight = platforms.Sum(p => Math.Max(1, p.Config.Weight));
        var target = Random.Shared.Next(1, totalWeight + 1);
        var running = 0;
        PlatformState? selected = null;
        foreach (var platform in platforms)
        {
            running += Math.Max(1, platform.Config.Weight);
            if (target <= running)
            {
                selected = platform;
                break;
            }
        }

        selected ??= platforms[0];
        var ordered = new List<PlatformState>(platforms.Count) { selected };
        ordered.AddRange(platforms.Where(p => p != selected).OrderBy(p => p.Config.Priority).ThenByDescending(p => p.Config.Weight));
        return ordered;
    }

    private static IReadOnlyList<PlatformState> OrderByPriority(List<PlatformState> platforms)
    {
        return platforms.OrderBy(p => p.Config.Priority).ThenByDescending(p => p.Config.Weight).ToList();
    }
}

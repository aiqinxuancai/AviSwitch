using EasySwitcher.Config;
using Spectre.Console;

namespace EasySwitcher.Services;

public static class StartupReporter
{
    public static void Print(AppConfig config, string configPath)
    {
        AnsiConsole.MarkupLine($"[green]EasySwitcher[/] loaded config: [blue]{Markup.Escape(configPath)}[/]");

        var table = new Table();
        table.AddColumn("Name");
        table.AddColumn("Group");
        table.AddColumn("Priority");
        table.AddColumn("Weight");
        table.AddColumn("Base URL");

        foreach (var platform in config.Platforms)
        {
            table.AddRow(
                Markup.Escape(platform.Name),
                Markup.Escape(platform.Group),
                platform.Priority.ToString(),
                platform.Weight.ToString(),
                Markup.Escape(platform.BaseUrl));
        }

        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine($"[grey]Listening:[/] {Markup.Escape(config.Server.Listen)}");
    }
}

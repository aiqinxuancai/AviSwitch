using EasySwitcher.Config;
using EasySwitcher.Runtime;
using EasySwitcher.Services;
using Spectre.Console;

var configPath = ConfigLoader.ResolvePath(args);
AppConfig config;
try
{
    config = ConfigLoader.Load(configPath);
}
catch (Exception ex)
{
    AnsiConsole.MarkupLine($"[red]Failed to load config:[/] {Markup.Escape(ex.Message)}");
    return;
}

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls(config.Server.Listen);

builder.Services.AddSingleton(config);
builder.Services.AddSingleton<PlatformRegistry>();
builder.Services.AddSingleton<HealthTracker>();
builder.Services.AddSingleton<LoadBalancer>();
builder.Services.AddSingleton<RequestLogger>();
builder.Services.AddSingleton<ProxyService>();

var app = builder.Build();

StartupReporter.Print(config, configPath);

app.Map("/{**catchall}", async (HttpContext context, ProxyService proxy) =>
{
    await proxy.HandleAsync(context);
});

app.Run();

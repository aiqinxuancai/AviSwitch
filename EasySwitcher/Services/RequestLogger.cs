using Spectre.Console;

namespace EasySwitcher.Services;

public sealed class RequestLogger
{
    public void Log(ProxyLogEntry entry)
    {
        var statusColor = entry.Success ? "green" : "red";
        var statusText = entry.Success ? "OK" : "FAIL";
        var attemptText = entry.AttemptLimit > 1 ? $" (attempt {entry.Attempt}/{entry.AttemptLimit})" : string.Empty;
        var message = $"[grey]{entry.Timestamp:O}[/] [blue]{Markup.Escape(entry.Group)}[/] [cyan]{Markup.Escape(entry.Platform)}[/] " +
                      $"{Markup.Escape(entry.Method)} {Markup.Escape(entry.PathAndQuery)} " +
                      $"[{statusColor}]{entry.StatusCode} {statusText}[/] {entry.ElapsedMilliseconds}ms{attemptText}";

        if (!string.IsNullOrWhiteSpace(entry.Error))
        {
            message += $" [red]{Markup.Escape(entry.Error)}[/]";
        }

        AnsiConsole.MarkupLine(message);
    }
}

public sealed record ProxyLogEntry(
    DateTimeOffset Timestamp,
    string Group,
    string Platform,
    string Method,
    string PathAndQuery,
    int StatusCode,
    long ElapsedMilliseconds,
    bool Success,
    int Attempt,
    int AttemptLimit,
    string? Error);

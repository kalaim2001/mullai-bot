namespace Mullai.TaskRuntime.Models;

public sealed record TaskResponseFeedItem
{
    public string TaskId { get; init; } = string.Empty;
    public string? SessionKey { get; init; }
    public string Response { get; init; } = string.Empty;
}

namespace Suprema.Core.Models;

public sealed class ContentPage
{
    public required string Slug { get; init; }
    public required string Title { get; init; }
    public required string MarkdownBody { get; init; }
    public string? MetaDescription { get; init; }
    public DateTime UpdatedUtc { get; init; }
}

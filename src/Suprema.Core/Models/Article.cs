namespace Suprema.Core.Models;

public sealed class Article
{
    public required string Slug { get; init; }
    public required string Title { get; init; }
    public required string Excerpt { get; init; }
    public required string Body { get; init; }
    public required string HeroImage { get; init; }
    public required DateTime PublishedUtc { get; init; }
    public string? Author { get; init; }
    public List<string> Tags { get; init; } = [];
}

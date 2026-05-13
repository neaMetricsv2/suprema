namespace Suprema.Core.Models;

public sealed class Product
{
    public required string Slug { get; init; }
    public required string CategorySlug { get; init; }
    public required string Name { get; init; }
    public required string TagLine { get; init; }
    public required string Summary { get; init; }
    public required string HeroImage { get; init; }
    public List<string> Gallery { get; init; } = [];
    public List<KeyFeature> Features { get; init; } = [];
    public List<SpecGroup> Specs { get; init; } = [];
    public string? DatasheetUrl { get; init; }
    public List<string> RelatedSlugs { get; init; } = [];
    public bool Featured { get; init; }
    public int SortOrder { get; init; }
    public DateTime UpdatedUtc { get; init; }
}

public sealed record KeyFeature(string Title, string Description, string? Icon);
public sealed record SpecGroup(string Title, Dictionary<string, string> Items);

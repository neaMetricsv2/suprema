namespace Suprema.Core.Models;

public sealed class Solution
{
    public required string Slug { get; init; }
    public required string Name { get; init; }
    public required string TagLine { get; init; }
    public required string Body { get; init; }
    public required string HeroImage { get; init; }
    public List<string> RelatedProductSlugs { get; init; } = [];
    public List<CaseStudyRef> CaseStudies { get; init; } = [];
    public int SortOrder { get; init; }
}

public sealed record CaseStudyRef(string Title, string Customer, string Summary, string? Url);

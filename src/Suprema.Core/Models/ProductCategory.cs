namespace Suprema.Core.Models;

public sealed class ProductCategory
{
    public required string Slug { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string HeroImage { get; init; }
    public string? Icon { get; init; }
    public int SortOrder { get; init; }
}

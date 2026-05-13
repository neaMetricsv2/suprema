using Suprema.Core.Models;

namespace Suprema.Web.ViewModels;

public sealed class HomeViewModel
{
    public required IReadOnlyList<Product> FeaturedProducts { get; init; }
    public required IReadOnlyList<ProductCategory> Categories { get; init; }
    public required IReadOnlyList<Solution> Solutions { get; init; }
    public required IReadOnlyList<Article> LatestArticles { get; init; }
}

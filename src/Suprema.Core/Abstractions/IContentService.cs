using Suprema.Core.Models;

namespace Suprema.Core.Abstractions;

public interface IContentService
{
    IReadOnlyList<ProductCategory> GetCategories();
    ProductCategory? GetCategory(string slug);

    IReadOnlyList<Product> GetAllProducts();
    IReadOnlyList<Product> GetProductsByCategory(string categorySlug);
    IReadOnlyList<Product> GetFeaturedProducts(int take = 4);
    Product? GetProduct(string categorySlug, string productSlug);

    IReadOnlyList<Solution> GetSolutions();
    Solution? GetSolution(string slug);

    IReadOnlyList<Article> GetArticles(int? take = null);
    Article? GetArticle(string slug);

    ContentPage? GetPage(string slug);
}

using System.Collections.Frozen;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Suprema.Core.Abstractions;
using Suprema.Core.Models;

namespace Suprema.Content.Services;

public sealed class ContentService : IContentService
{
    private readonly FrozenDictionary<string, ProductCategory> _categories;
    private readonly FrozenDictionary<string, Product> _products;        // key: "{category}/{slug}"
    private readonly FrozenDictionary<string, Solution> _solutions;
    private readonly FrozenDictionary<string, Article> _articles;
    private readonly FrozenDictionary<string, ContentPage> _pages;

    public ContentService(string contentRoot, ILogger<ContentService> logger)
    {
        var opts = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };

        _categories = LoadCategories(contentRoot, opts);
        _products   = LoadProducts(contentRoot, opts, _categories, logger);
        _solutions  = LoadSolutions(contentRoot, opts);
        _articles   = LoadArticles(contentRoot, opts);
        _pages      = LoadPages(contentRoot);

        logger.LogInformation(
            "Content loaded — categories: {Categories}, products: {Products}, solutions: {Solutions}, articles: {Articles}, pages: {Pages}",
            _categories.Count, _products.Count, _solutions.Count, _articles.Count, _pages.Count);
    }

    // --- Public API ---------------------------------------------------------

    public IReadOnlyList<ProductCategory> GetCategories()
        => [.. _categories.Values.OrderBy(c => c.SortOrder)];

    public ProductCategory? GetCategory(string slug)
        => _categories.GetValueOrDefault(slug);

    public IReadOnlyList<Product> GetAllProducts()
        => [.. _products.Values.OrderBy(p => p.SortOrder)];

    public IReadOnlyList<Product> GetProductsByCategory(string categorySlug)
        => [.. _products.Values.Where(p => p.CategorySlug == categorySlug).OrderBy(p => p.SortOrder)];

    public IReadOnlyList<Product> GetFeaturedProducts(int take = 4)
        => [.. _products.Values.Where(p => p.Featured).OrderBy(p => p.SortOrder).Take(take)];

    public Product? GetProduct(string categorySlug, string productSlug)
        => _products.GetValueOrDefault($"{categorySlug}/{productSlug}");

    public IReadOnlyList<Solution> GetSolutions()
        => [.. _solutions.Values.OrderBy(s => s.SortOrder)];

    public Solution? GetSolution(string slug)
        => _solutions.GetValueOrDefault(slug);

    public IReadOnlyList<Article> GetArticles(int? take = null)
    {
        var ordered = _articles.Values.OrderByDescending(a => a.PublishedUtc);
        return take.HasValue ? [.. ordered.Take(take.Value)] : [.. ordered];
    }

    public Article? GetArticle(string slug) => _articles.GetValueOrDefault(slug);

    public ContentPage? GetPage(string slug) => _pages.GetValueOrDefault(slug);

    // --- Loaders ------------------------------------------------------------

    private static FrozenDictionary<string, ProductCategory> LoadCategories(
        string root, JsonSerializerOptions opts)
    {
        var productsRoot = Path.Combine(root, "products");
        var dict = new Dictionary<string, ProductCategory>(StringComparer.OrdinalIgnoreCase);

        foreach (var (_, category) in JsonContentLoader.LoadSubDirectories<ProductCategory>(
            productsRoot, opts, f => Path.GetFileName(f).Equals("_category.json", StringComparison.OrdinalIgnoreCase)))
        {
            if (dict.ContainsKey(category.Slug))
                throw new InvalidOperationException($"Duplicate category slug: '{category.Slug}'");
            dict[category.Slug] = category;
        }

        return dict.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
    }

    private static FrozenDictionary<string, Product> LoadProducts(
        string root, JsonSerializerOptions opts,
        FrozenDictionary<string, ProductCategory> categories,
        ILogger logger)
    {
        var productsRoot = Path.Combine(root, "products");
        var dict = new Dictionary<string, Product>(StringComparer.OrdinalIgnoreCase);

        foreach (var (subDir, product) in JsonContentLoader.LoadSubDirectories<Product>(
            productsRoot, opts,
            f => !Path.GetFileName(f).Equals("_category.json", StringComparison.OrdinalIgnoreCase)))
        {
            // Directory name is authoritative for categorySlug
            var canonical = product;
            if (!string.Equals(product.CategorySlug, subDir, StringComparison.OrdinalIgnoreCase))
            {
                logger.LogWarning(
                    "Product '{Slug}' categorySlug '{JsonCat}' overridden by directory name '{DirCat}'",
                    product.Slug, product.CategorySlug, subDir);

                canonical = new Product
                {
                    Slug           = product.Slug,
                    CategorySlug   = subDir,
                    Name           = product.Name,
                    TagLine        = product.TagLine,
                    Summary        = product.Summary,
                    HeroImage      = product.HeroImage,
                    Gallery        = product.Gallery,
                    Features       = product.Features,
                    Specs          = product.Specs,
                    DatasheetUrl   = product.DatasheetUrl,
                    RelatedSlugs   = product.RelatedSlugs,
                    Featured       = product.Featured,
                    SortOrder      = product.SortOrder,
                    UpdatedUtc     = product.UpdatedUtc
                };
            }

            var key = $"{subDir}/{canonical.Slug}";

            if (dict.ContainsKey(key))
                throw new InvalidOperationException($"Duplicate product key: '{key}'");
            dict[key] = canonical;
        }

        return dict.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
    }

    private static FrozenDictionary<string, Solution> LoadSolutions(
        string root, JsonSerializerOptions opts)
    {
        var solutionsDir = Path.Combine(root, "solutions");
        var dict = new Dictionary<string, Solution>(StringComparer.OrdinalIgnoreCase);

        foreach (var solution in JsonContentLoader.LoadDirectory<Solution>(solutionsDir, opts))
        {
            if (dict.ContainsKey(solution.Slug))
                throw new InvalidOperationException($"Duplicate solution slug: '{solution.Slug}'");
            dict[solution.Slug] = solution;
        }

        return dict.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
    }

    private static FrozenDictionary<string, Article> LoadArticles(
        string root, JsonSerializerOptions opts)
    {
        var articlesDir = Path.Combine(root, "articles");
        var dict = new Dictionary<string, Article>(StringComparer.OrdinalIgnoreCase);

        foreach (var article in JsonContentLoader.LoadDirectory<Article>(articlesDir, opts))
        {
            if (dict.ContainsKey(article.Slug))
                throw new InvalidOperationException($"Duplicate article slug: '{article.Slug}'");
            dict[article.Slug] = article;
        }

        return dict.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
    }

    private static FrozenDictionary<string, ContentPage> LoadPages(string root)
    {
        var pagesDir = Path.Combine(root, "pages");
        var dict = new Dictionary<string, ContentPage>(StringComparer.OrdinalIgnoreCase);

        if (!Directory.Exists(pagesDir))
            return dict.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

        foreach (var file in Directory.EnumerateFiles(pagesDir, "*.md", SearchOption.TopDirectoryOnly))
        {
            var slug = Path.GetFileNameWithoutExtension(file);
            var rawContent = File.ReadAllText(file);
            var (meta, body) = MarkdownContentLoader.ParseFrontMatter(rawContent);

            var page = new ContentPage
            {
                Slug           = slug,
                Title          = meta.GetValueOrDefault("title", slug),
                MarkdownBody   = body,
                MetaDescription = meta.GetValueOrDefault("metaDescription"),
                UpdatedUtc     = meta.TryGetValue("updatedUtc", out var d) && DateTime.TryParse(d, out var dt)
                                    ? dt.ToUniversalTime()
                                    : DateTime.UtcNow
            };

            if (dict.ContainsKey(slug))
                throw new InvalidOperationException($"Duplicate page slug: '{slug}'");
            dict[slug] = page;
        }

        return dict.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
    }
}

# Suprema South Africa — Localized Partner Site

**Repo:** https://github.com/neaMetricsv2/suprema  
**Domain:** suprema.co.za  
**Owner:** Tshepo Tlou  
**Stack:** .NET 10 / ASP.NET Core MVC  
**Status:** Phase 1–3 in a 2-day timebox. Phase 4+ later.

This document is the full handover. Read top to bottom before touching code.

---

## 1. Project Brief

Build a Suprema-branded marketing site for the South African market under **suprema.co.za**. This is *not* a reseller catalogue with our own branding — it's an authorised, localised mirror of Suprema's global site, adapted for SA visitors. Suprema has given written approval; partner asset kit (logos, product imagery, datasheets, approved web copy) is in hand.

**Visitors should see:**

- Suprema brand and product information (from approved assets)
- ZA-specific contact details, support hours, regulatory references (POPIA, not GDPR)
- Local case studies and partner contact paths
- English (en-ZA) only for v1; i18n plumbing in place for af-ZA / zu-ZA later

**Visitors should not see:**

- Global investor pages, non-African regional content, pricing (B2B → "request a quote")
- Anything that isn't in the approved partner asset kit

---

## 2. Constraints & Decisions

| Area | Decision | Rationale |
|---|---|---|
| Languages v1 | en-ZA only | Avoid half-translated content. Plumb i18n; translate later. |
| CMS | **None for v1** — content in repo as JSON + Markdown | Two-day timebox. Migrate to CMS in Phase 4 without changing controllers. |
| Database | **None for marketing content** | Static + cached. Contact form posts go to a stub endpoint, wired to CRM later. |
| Framework | ASP.NET Core MVC (.NET 10), Razor views | Matches AccessManager / Connect stack; team velocity. |
| Frontend | Bootstrap 5 baseline, restyled within Suprema brand guidelines | Speed. Custom design system is Phase 4. |
| Hosting | Azure South Africa North (target) | Latency for ZA visitors. Cloudflare in front later. |
| Auth | None for v1 | No partner login yet; link out to Suprema global support. |
| Analytics | Plumbing only (GA4 placeholder behind cookie consent) | POPIA-compliant from day one. |

### Naming convention

Mirror the `Connect.*` and `AccessManager.*` pattern:

- `Suprema.Core` — models, interfaces, enums. No external dependencies.
- `Suprema.Content` — content loaders, caching, JSON/MD data files.
- `Suprema.Web` — MVC frontend, controllers, Razor views, `wwwroot`.

(`Suprema.Api` and `Suprema.Worker` are *not* needed for v1 — add later if/when forms route to a back-end service.)

---

## 3. Solution Scaffold

```bash
mkdir -p src && cd src
dotnet new sln -n Suprema

dotnet new classlib -n Suprema.Core    -f net10.0
dotnet new classlib -n Suprema.Content -f net10.0
dotnet new mvc      -n Suprema.Web     -f net10.0

dotnet sln add Suprema.Core Suprema.Content Suprema.Web
dotnet add Suprema.Web     reference Suprema.Core Suprema.Content
dotnet add Suprema.Content reference Suprema.Core

# Packages
dotnet add Suprema.Content package Markdig
dotnet add Suprema.Web     package Microsoft.AspNetCore.Mvc.Localization
```

### Folder tree

```
suprema/
├── README.md
├── SUPREMA_ZA_BUILD_PLAN.md       ← this file
├── src/
│   ├── Suprema.sln
│   │
│   ├── Suprema.Core/
│   │   ├── Suprema.Core.csproj
│   │   ├── Models/
│   │   │   ├── Product.cs
│   │   │   ├── ProductCategory.cs
│   │   │   ├── Solution.cs
│   │   │   ├── Article.cs
│   │   │   ├── ContentPage.cs
│   │   │   └── KeyFeature.cs
│   │   ├── Abstractions/
│   │   │   ├── IContentService.cs
│   │   │   └── IContentLoader.cs
│   │   └── Enums/
│   │       └── PageType.cs
│   │
│   ├── Suprema.Content/
│   │   ├── Suprema.Content.csproj
│   │   ├── Services/
│   │   │   ├── ContentService.cs
│   │   │   ├── JsonContentLoader.cs
│   │   │   └── MarkdownContentLoader.cs
│   │   ├── Extensions/
│   │   │   └── ServiceCollectionExtensions.cs
│   │   └── Data/
│   │       ├── products/
│   │       │   ├── access-control/
│   │       │   │   ├── _category.json
│   │       │   │   ├── xstation-2.json
│   │       │   │   └── biostation-3.json
│   │       │   ├── time-attendance/
│   │       │   │   └── _category.json
│   │       │   └── biometrics/
│   │       │       └── _category.json
│   │       ├── solutions/
│   │       │   ├── enterprise.json
│   │       │   ├── healthcare.json
│   │       │   ├── retail.json
│   │       │   └── education.json
│   │       ├── articles/
│   │       │   └── 2026-01-launch-suprema-za.json
│   │       └── pages/
│   │           ├── about.md
│   │           ├── privacy-policy.md
│   │           ├── cookie-policy.md
│   │           ├── paia-manual.md
│   │           └── terms.md
│   │
│   └── Suprema.Web/
│       ├── Suprema.Web.csproj
│       ├── Program.cs
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       ├── Controllers/
│       │   ├── HomeController.cs
│       │   ├── ProductsController.cs
│       │   ├── SolutionsController.cs
│       │   ├── NewsController.cs
│       │   ├── ContactController.cs
│       │   ├── PagesController.cs
│       │   └── LegalController.cs
│       ├── ViewModels/
│       │   ├── HomeViewModel.cs
│       │   ├── ProductCategoryViewModel.cs
│       │   ├── ProductDetailViewModel.cs
│       │   ├── ContactFormViewModel.cs
│       │   └── ContentPageViewModel.cs
│       ├── Views/
│       │   ├── Shared/
│       │   │   ├── _Layout.cshtml
│       │   │   ├── _Header.cshtml
│       │   │   ├── _Footer.cshtml
│       │   │   ├── _CookieConsent.cshtml
│       │   │   ├── _Meta.cshtml
│       │   │   └── _ValidationScriptsPartial.cshtml
│       │   ├── Home/Index.cshtml
│       │   ├── Products/
│       │   │   ├── Index.cshtml         (all categories)
│       │   │   ├── Category.cshtml      (one category)
│       │   │   └── Detail.cshtml        (one product)
│       │   ├── Solutions/
│       │   │   ├── Index.cshtml
│       │   │   └── Detail.cshtml
│       │   ├── News/
│       │   │   ├── Index.cshtml
│       │   │   └── Article.cshtml
│       │   ├── Contact/Index.cshtml
│       │   ├── Pages/Show.cshtml
│       │   └── Legal/Show.cshtml
│       ├── Resources/
│       │   ├── SharedResources.resx
│       │   └── SharedResources.en-ZA.resx
│       └── wwwroot/
│           ├── assets/
│           │   ├── logos/
│           │   ├── products/{slug}/
│           │   └── datasheets/
│           ├── css/site.css
│           ├── js/site.js
│           └── favicon.ico
```

---

## 4. Core Models (`Suprema.Core`)

All models are immutable POCOs. Init-only properties, `required` where appropriate, collection defaults to empty.

```csharp
// Models/Product.cs
namespace Suprema.Core.Models;

public sealed class Product
{
    public required string Slug { get; init; }
    public required string CategorySlug { get; init; }
    public required string Name { get; init; }
    public required string TagLine { get; init; }
    public required string Summary { get; init; }       // 1–2 paragraphs of approved copy
    public required string HeroImage { get; init; }     // /assets/products/{slug}/hero.jpg
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
```

```csharp
// Models/ProductCategory.cs
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
```

```csharp
// Models/Solution.cs
namespace Suprema.Core.Models;

public sealed class Solution
{
    public required string Slug { get; init; }
    public required string Name { get; init; }           // e.g. "Healthcare"
    public required string TagLine { get; init; }
    public required string Body { get; init; }           // Markdown; renders via Markdig
    public required string HeroImage { get; init; }
    public List<string> RelatedProductSlugs { get; init; } = [];
    public List<CaseStudyRef> CaseStudies { get; init; } = [];
    public int SortOrder { get; init; }
}

public sealed record CaseStudyRef(string Title, string Customer, string Summary, string? Url);
```

```csharp
// Models/Article.cs
namespace Suprema.Core.Models;

public sealed class Article
{
    public required string Slug { get; init; }
    public required string Title { get; init; }
    public required string Excerpt { get; init; }
    public required string Body { get; init; }           // Markdown
    public required string HeroImage { get; init; }
    public required DateTime PublishedUtc { get; init; }
    public string? Author { get; init; }
    public List<string> Tags { get; init; } = [];
}
```

```csharp
// Models/ContentPage.cs
namespace Suprema.Core.Models;

public sealed class ContentPage
{
    public required string Slug { get; init; }
    public required string Title { get; init; }
    public required string MarkdownBody { get; init; }
    public string? MetaDescription { get; init; }
    public DateTime UpdatedUtc { get; init; }
}
```

```csharp
// Abstractions/IContentService.cs
namespace Suprema.Core.Abstractions;
using Suprema.Core.Models;

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
```

---

## 5. Content Service (`Suprema.Content`)

Load everything once at startup, cache in memory as immutable dictionaries. Zero I/O on the request path.

```csharp
// Services/ContentService.cs
using System.Collections.Frozen;
using System.Text.Json;
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

    public ContentService(string contentRoot)
    {
        var opts = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };

        _categories = LoadCategories(contentRoot, opts);
        _products   = LoadProducts(contentRoot, opts);
        _solutions  = LoadSolutions(contentRoot, opts);
        _articles   = LoadArticles(contentRoot, opts);
        _pages      = LoadPages(contentRoot);
    }

    // --- Public API -----------------------------------------------------

    public IReadOnlyList<ProductCategory> GetCategories()
        => _categories.Values.OrderBy(c => c.SortOrder).ToList();

    public ProductCategory? GetCategory(string slug)
        => _categories.GetValueOrDefault(slug);

    public IReadOnlyList<Product> GetAllProducts()
        => _products.Values.OrderBy(p => p.SortOrder).ToList();

    public IReadOnlyList<Product> GetProductsByCategory(string categorySlug)
        => _products.Values
            .Where(p => p.CategorySlug == categorySlug)
            .OrderBy(p => p.SortOrder)
            .ToList();

    public IReadOnlyList<Product> GetFeaturedProducts(int take = 4)
        => _products.Values
            .Where(p => p.Featured)
            .OrderBy(p => p.SortOrder)
            .Take(take)
            .ToList();

    public Product? GetProduct(string categorySlug, string productSlug)
        => _products.GetValueOrDefault($"{categorySlug}/{productSlug}");

    public IReadOnlyList<Solution> GetSolutions()
        => _solutions.Values.OrderBy(s => s.SortOrder).ToList();

    public Solution? GetSolution(string slug)
        => _solutions.GetValueOrDefault(slug);

    public IReadOnlyList<Article> GetArticles(int? take = null)
    {
        var ordered = _articles.Values.OrderByDescending(a => a.PublishedUtc);
        return take.HasValue ? ordered.Take(take.Value).ToList() : ordered.ToList();
    }

    public Article? GetArticle(string slug) => _articles.GetValueOrDefault(slug);

    public ContentPage? GetPage(string slug) => _pages.GetValueOrDefault(slug);

    // --- Loaders --------------------------------------------------------
    // (Implementation: enumerate files under contentRoot/{products,solutions,...},
    //  deserialize JSON via System.Text.Json, parse Markdown pages with Markdig,
    //  return as FrozenDictionary. Throw on duplicate slugs.)

    private static FrozenDictionary<string, ProductCategory> LoadCategories(
        string root, JsonSerializerOptions opts) => /* TODO */ throw new NotImplementedException();

    private static FrozenDictionary<string, Product> LoadProducts(
        string root, JsonSerializerOptions opts) => /* TODO */ throw new NotImplementedException();

    private static FrozenDictionary<string, Solution> LoadSolutions(
        string root, JsonSerializerOptions opts) => /* TODO */ throw new NotImplementedException();

    private static FrozenDictionary<string, Article> LoadArticles(
        string root, JsonSerializerOptions opts) => /* TODO */ throw new NotImplementedException();

    private static FrozenDictionary<string, ContentPage> LoadPages(
        string root) => /* TODO */ throw new NotImplementedException();
}
```

```csharp
// Extensions/ServiceCollectionExtensions.cs
using Microsoft.Extensions.DependencyInjection;
using Suprema.Core.Abstractions;
using Suprema.Content.Services;

namespace Suprema.Content.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSupremaContent(this IServiceCollection services, string contentRoot)
    {
        services.AddSingleton<IContentService>(_ => new ContentService(contentRoot));
        return services;
    }
}
```

**Note for Claude Code:** the loaders need to enumerate all `*.json` files (except `_category.json` for products), deserialize each, and assemble. For products, the directory name *is* the category slug — derive it during load, don't trust the JSON field if they conflict (log a warning).

---

## 6. Program.cs

```csharp
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Suprema.Content.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

builder.Services.AddLocalization(o => o.ResourcesPath = "Resources");

var contentRoot = Path.Combine(builder.Environment.ContentRootPath, "..", "Suprema.Content", "Data");
builder.Services.AddSupremaContent(Path.GetFullPath(contentRoot));

// Localization plumbing — en-ZA only for v1, but ready for af-ZA / zu-ZA
var supportedCultures = new[] { new CultureInfo("en-ZA") };
builder.Services.Configure<RequestLocalizationOptions>(o =>
{
    o.DefaultRequestCulture = new RequestCulture("en-ZA");
    o.SupportedCultures = supportedCultures;
    o.SupportedUICultures = supportedCultures;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseRequestLocalization();
app.UseAuthorization();

app.MapControllerRoute(
    name: "product-detail",
    pattern: "products/{category}/{slug}",
    defaults: new { controller = "Products", action = "Detail" });

app.MapControllerRoute(
    name: "product-category",
    pattern: "products/{category}",
    defaults: new { controller = "Products", action = "Category" });

app.MapControllerRoute(
    name: "solution-detail",
    pattern: "solutions/{slug}",
    defaults: new { controller = "Solutions", action = "Detail" });

app.MapControllerRoute(
    name: "article",
    pattern: "news/{slug}",
    defaults: new { controller = "News", action = "Article" });

app.MapControllerRoute(
    name: "legal",
    pattern: "legal/{slug}",
    defaults: new { controller = "Legal", action = "Show" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
```

---

## 7. Routing Map

| Path | Controller / Action | View |
|---|---|---|
| `/` | Home / Index | `Home/Index.cshtml` |
| `/products` | Products / Index | `Products/Index.cshtml` |
| `/products/access-control` | Products / Category | `Products/Category.cshtml` |
| `/products/access-control/xstation-2` | Products / Detail | `Products/Detail.cshtml` |
| `/solutions` | Solutions / Index | `Solutions/Index.cshtml` |
| `/solutions/healthcare` | Solutions / Detail | `Solutions/Detail.cshtml` |
| `/news` | News / Index | `News/Index.cshtml` |
| `/news/{slug}` | News / Article | `News/Article.cshtml` |
| `/about` | Pages / Show (slug=about) | `Pages/Show.cshtml` |
| `/contact` | Contact / Index | `Contact/Index.cshtml` |
| `/legal/privacy-policy` | Legal / Show | `Legal/Show.cshtml` |
| `/legal/cookie-policy` | Legal / Show | `Legal/Show.cshtml` |
| `/legal/paia-manual` | Legal / Show | `Legal/Show.cshtml` |
| `/legal/terms` | Legal / Show | `Legal/Show.cshtml` |

---

## 8. Controllers (skeletons)

```csharp
// Controllers/HomeController.cs
using Microsoft.AspNetCore.Mvc;
using Suprema.Core.Abstractions;
using Suprema.Web.ViewModels;

namespace Suprema.Web.Controllers;

public sealed class HomeController(IContentService content) : Controller
{
    public IActionResult Index() => View(new HomeViewModel
    {
        FeaturedProducts = content.GetFeaturedProducts(4),
        Categories       = content.GetCategories(),
        Solutions        = content.GetSolutions(),
        LatestArticles   = content.GetArticles(3)
    });

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();
}
```

```csharp
// Controllers/ProductsController.cs
using Microsoft.AspNetCore.Mvc;
using Suprema.Core.Abstractions;

namespace Suprema.Web.Controllers;

public sealed class ProductsController(IContentService content) : Controller
{
    public IActionResult Index() => View(content.GetCategories());

    public IActionResult Category(string category)
    {
        var cat = content.GetCategory(category);
        if (cat is null) return NotFound();
        ViewData["Category"] = cat;
        return View(content.GetProductsByCategory(category));
    }

    public IActionResult Detail(string category, string slug)
    {
        var product = content.GetProduct(category, slug);
        return product is null ? NotFound() : View(product);
    }
}
```

```csharp
// Controllers/ContactController.cs
using Microsoft.AspNetCore.Mvc;
using Suprema.Web.ViewModels;

namespace Suprema.Web.Controllers;

public sealed class ContactController : Controller
{
    public IActionResult Index() => View(new ContactFormViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Index(ContactFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        // TODO: route to CRM / mailbox. For v1, log + return ThankYou view.
        // Wire to AccessManager lead pipeline OR a dedicated SMTP send in Phase 4.

        return RedirectToAction(nameof(ThankYou));
    }

    public IActionResult ThankYou() => View();
}
```

---

## 9. Layout & Partials

`_Layout.cshtml` must include:

- `<html lang="en-ZA">`
- `<meta name="viewport" content="width=device-width, initial-scale=1">`
- `<link rel="alternate" hreflang="en-ZA" href="https://suprema.co.za@(Context.Request.Path)">`
- `og:locale="en_ZA"`, `og:site_name="Suprema South Africa"`
- A `_CookieConsent.cshtml` partial rendered just before `</body>` — POPIA-compliant, default to *decline*, no analytics scripts fire until consent is given.

`_Footer.cshtml` must include:

- ZA company registration number, VAT number, physical address (Joburg / wherever)
- ZA phone + email + support hours in SAST
- Links: Privacy Policy, Cookie Policy, PAIA Manual, Terms, Contact
- Copyright line referencing both the local entity and Suprema where appropriate (confirm wording with Suprema brand team)

---

## 10. Localization Setup

For v1, the plumbing is in place but only `en-ZA` is wired:

- `Resources/SharedResources.resx` — neutral, English source strings
- `Resources/SharedResources.en-ZA.resx` — copy (allows ZA overrides like "colour" / SA-specific phrasing)
- Inject `IStringLocalizer<SharedResources>` into views and controllers
- All UI strings (nav, buttons, form labels) go through the localizer — *content* (product copy, articles) stays in JSON/MD

**Adding af-ZA / zu-ZA later** = add `SharedResources.af-ZA.resx` + `SharedResources.zu-ZA.resx`, plus translated copies of the JSON/MD content trees under `Data/af-ZA/...`. The `ContentService` will need a culture-aware overload at that point. Don't build it now.

---

## 11. POPIA / Legal Pages

These must exist as Markdown under `Suprema.Content/Data/pages/` for v1, served by `LegalController`:

- `privacy-policy.md` — POPIA-compliant. Covers: information officer, lawful basis, retention, data subject rights (sections 23–25 of POPIA), cross-border transfer (Suprema HQ is in Korea — flag this), cookies.
- `cookie-policy.md` — Categories: strictly necessary, analytics, marketing. Default opt-out.
- `paia-manual.md` — Required under the Promotion of Access to Information Act. Template available from the SA Human Rights Commission.
- `terms.md` — Site T&Cs (separate from any product / partnership T&Cs).

**TODO before launch:** all four must be reviewed by legal counsel. The Markdown placeholders ship with `STATUS: DRAFT — REVIEW REQUIRED` at the top.

**Information Officer:** must be a registered SA-resident natural person under POPIA. Decide who.

**Cookie consent flow:**

1. First visit → banner appears, "Accept all" / "Reject non-essential" / "Customise"
2. Choice persisted in a first-party cookie (`consent.v1`, 12-month expiry)
3. Analytics scripts gated on `consent.analytics === true`
4. Re-show banner if consent cookie missing or schema version changed

---

## 12. Content Inventory

Template lives at the repo root as `content-inventory.xlsx` (or `.csv` for diff-friendliness). Columns:

| # | Source URL | Page Type | SA Title | Slug | Action | SA Notes | Assets Needed | Approver | Status |
|---|---|---|---|---|---|---|---|---|---|

**Initial target rows for v1** (≤40 total):

- Home (1)
- Product categories: Access Control, Time & Attendance, Biometrics (3)
- Product detail pages: ~10 flagship devices across the three categories
- Solutions / Industries: Enterprise, Healthcare, Retail, Education, Government (5)
- About (1)
- Contact (1)
- News index + 2–3 launch articles (3–4)
- Legal: Privacy, Cookies, PAIA, Terms (4)

Anything outside this list is **v2** — log it in the inventory with Action = "Drop (v2)".

---

## 13. Sample Content Files

All sample content below uses **placeholder text**. Replace with the approved copy from the Suprema partner asset kit before going live. Do not paraphrase scraped content from the global site — use only what's in the approved kit.

### `Data/products/access-control/_category.json`

```json
{
  "slug": "access-control",
  "name": "Access Control",
  "description": "PLACEHOLDER — replace with approved category description from Suprema partner kit.",
  "heroImage": "/assets/products/_categories/access-control.jpg",
  "icon": "shield",
  "sortOrder": 1
}
```

### `Data/products/access-control/xstation-2.json`

```json
{
  "slug": "xstation-2",
  "categorySlug": "access-control",
  "name": "XStation 2",
  "tagLine": "PLACEHOLDER — short tagline from approved copy.",
  "summary": "PLACEHOLDER — 1–2 paragraph product summary from the approved partner kit. Do not write original copy here.",
  "heroImage": "/assets/products/xstation-2/hero.jpg",
  "gallery": [
    "/assets/products/xstation-2/gallery-01.jpg",
    "/assets/products/xstation-2/gallery-02.jpg"
  ],
  "features": [
    { "title": "PLACEHOLDER feature 1", "description": "From approved kit.", "icon": "fingerprint" },
    { "title": "PLACEHOLDER feature 2", "description": "From approved kit.", "icon": "wifi" }
  ],
  "specs": [
    {
      "title": "General",
      "items": {
        "CPU": "PLACEHOLDER",
        "Memory": "PLACEHOLDER",
        "Operating Temperature": "PLACEHOLDER"
      }
    },
    {
      "title": "Connectivity",
      "items": {
        "Ethernet": "PLACEHOLDER",
        "Wi-Fi": "PLACEHOLDER"
      }
    }
  ],
  "datasheetUrl": "/assets/datasheets/xstation-2.pdf",
  "relatedSlugs": ["biostation-3"],
  "featured": true,
  "sortOrder": 10,
  "updatedUtc": "2026-05-01T00:00:00Z"
}
```

### `Data/solutions/healthcare.json`

```json
{
  "slug": "healthcare",
  "name": "Healthcare",
  "tagLine": "PLACEHOLDER — short solution tagline.",
  "body": "PLACEHOLDER markdown body. Replace with approved healthcare solution copy from the partner kit. Multiple paragraphs allowed.\n\nSecond paragraph here.",
  "heroImage": "/assets/solutions/healthcare/hero.jpg",
  "relatedProductSlugs": ["xstation-2", "biostation-3"],
  "caseStudies": [
    {
      "title": "PLACEHOLDER case study title",
      "customer": "PLACEHOLDER customer name (with written permission)",
      "summary": "1–2 sentence summary of the deployment and outcome.",
      "url": null
    }
  ],
  "sortOrder": 2
}
```

### `Data/pages/about.md`

```markdown
---
title: "About Suprema South Africa"
metaDescription: "PLACEHOLDER meta description, ~150 chars."
updatedUtc: "2026-05-01"
---

# About Suprema South Africa

PLACEHOLDER intro paragraph — replace with approved copy describing the local
entity, its relationship with Suprema Inc., and the SA market focus.

## Our local presence

PLACEHOLDER — Joburg / Cape Town / Durban offices, support reach, etc.

## Contact

See the [Contact page](/contact).
```

### `Data/pages/privacy-policy.md` (stub)

```markdown
---
title: "Privacy Policy"
metaDescription: "How Suprema South Africa collects, uses, and protects your personal information under POPIA."
updatedUtc: "2026-05-01"
status: "DRAFT — REVIEW REQUIRED"
---

# Privacy Policy

**Status: DRAFT. Must be reviewed by legal counsel before launch.**

## 1. Who we are

PLACEHOLDER — local entity legal name, registration number, registered address,
information officer name and contact.

## 2. What we collect

...

## 3. Lawful basis (POPIA s.11)

...

## 4. Your rights (POPIA s.23–25)

...

## 5. Cross-border transfer

PLACEHOLDER — Suprema Inc. is headquartered outside SA. Describe the transfer
mechanism (consent / contractual safeguards / adequacy).

## 6. Cookies

See the [Cookie Policy](/legal/cookie-policy).

## 7. Contact the Information Officer

PLACEHOLDER — name, email, postal address.
```

---

## 14. Day-by-Day Schedule (2 days)

### Day 1 — Phase 1 + Phase 2 scaffold

**Morning (~4h)** — Content inventory + asset organisation
- [ ] Walk supremainc.com top nav, log every page in `content-inventory.xlsx`
- [ ] Mark Keep / Localize / Replace / Drop, target ≤40 rows for v1
- [ ] Organise the partner asset kit into `wwwroot/assets/{logos,products,datasheets}/`
- [ ] For each product to ship in v1, copy the approved imagery and datasheet PDF into the right folder

**Afternoon (~4h)** — Solution scaffold + content service
- [ ] Run the `dotnet new` commands from §3
- [ ] Drop in the Core models from §4
- [ ] Implement `JsonContentLoader`, `MarkdownContentLoader`, and the `ContentService` loaders (the `TODO` bodies in §5)
- [ ] Wire DI in `Program.cs` (§6)
- [ ] Seed `_category.json` for each category and 1 product JSON file to prove the pipeline end-to-end
- [ ] `dotnet run` → confirm content loads (log category + product counts at startup)

### Day 2 — Templates + i18n + ZA bits

**Morning (~4h)** — Controllers, views, routing
- [ ] Implement all controllers (§8)
- [ ] Build all Razor views — keep styling minimal (Bootstrap defaults), structure over polish
- [ ] `_Layout.cshtml`, `_Header.cshtml`, `_Footer.cshtml` with hreflang + `og:locale`
- [ ] Plug remaining product / solution JSON files (the rest of the v1 inventory)
- [ ] Contact form posts to stub action, returns ThankYou view

**Afternoon (~4h)** — Localization plumbing + POPIA + polish
- [ ] `Resources/SharedResources.resx` + `.en-ZA.resx`, swap all hardcoded UI strings to localizer
- [ ] `RequestLocalizationOptions` (already in §6 Program.cs)
- [ ] `_CookieConsent.cshtml` partial, default decline, persists choice in `consent.v1` cookie
- [ ] Stub all four legal pages from §13 with DRAFT status
- [ ] Footer with ZA contact details, registration number placeholder, SAST support hours
- [ ] README at repo root pointing back at this file
- [ ] Push to `neaMetricsv2/suprema`, tag `v0.1-scaffold`

---

## 15. Out of Scope for v1 (log as Phase 4+)

- CMS (headless or Umbraco) — content stays in repo
- Multilingual content (af-ZA / zu-ZA) — plumbing only
- Custom design system — Bootstrap defaults are fine for v1
- News/article publishing UI — devs add JSON files for now
- Partner login / portal
- Support portal — link out to Suprema global
- Live chat / chatbot
- SEO polish beyond basic meta tags and hreflang
- Analytics dashboards
- CI/CD — local dev only for the 2-day sprint
- Sitemap.xml / robots.txt — generate in Phase 4
- Schema.org JSON-LD product markup — Phase 4 SEO pass
- Search functionality
- ZAR pricing — B2B path is "request a quote" only

---

## 16. Phase 4 — CMS Migration Path

When marketing wants to edit content without devs, swap the `IContentService` implementation. Controllers and views do not change.

**Recommended target:** Umbraco CMS (full or Heartcore). It's .NET-native, runs on the same stack as AccessManager and Connect, and integrates cleanly with Azure SA North.

**Migration steps (sketch):**

1. Stand up Umbraco in a new project `Suprema.Cms`
2. Model Document Types matching the Core models (Product, ProductCategory, Solution, Article, ContentPage)
3. Build a one-off importer that reads the JSON/MD tree and creates Umbraco nodes
4. Implement `IContentService` as `UmbracoContentService`, register in DI instead of the file-based one
5. Add culture variants on Document Types when af-ZA / zu-ZA come online

---

## 17. Open Questions / TODOs for Tshepo

- [ ] **Information Officer** under POPIA — who is the named person? Required before privacy policy can go beyond DRAFT.
- [ ] **Local entity legal details** — registration number, VAT number, registered address, for the footer and privacy policy.
- [ ] **Suprema brand approval contact** — named contact for sign-off on each major content push.
- [ ] **CRM / lead destination** for contact form submissions — AccessManager pipeline, dedicated mailbox, or something else?
- [ ] **Hosting environment** — Azure SA North subscription confirmed? App Service plan size?
- [ ] **DNS** — who controls `suprema.co.za`? Need access to set up A / CNAME + email records.
- [ ] **Email** — `info@suprema.co.za`, `support@suprema.co.za` — provisioning owner.
- [ ] **Languages decision deadline** — when does af-ZA / zu-ZA need to ship (so translation can be commissioned)?
- [ ] **Case study permissions** — which SA customers can be named? Requires written consent per customer.
- [ ] **Pricing display policy** — confirm v1 is "request a quote" only.
- [ ] **Datasheet hosting** — serve from `wwwroot/assets/datasheets/` or link out to Suprema global? Affects asset sync workflow.

---

## 18. Conventions

- C# 13 / .NET 10
- File-scoped namespaces
- `sealed` classes by default
- `required` init-only properties on models
- Primary constructors on controllers
- Collection expressions `[]` for empty defaults
- `System.Text.Json` (not Newtonsoft)
- `FrozenDictionary` for read-only lookups built once at startup
- Async only where I/O is actually async — no `async` on pure-CPU paths
- No DB for marketing content. None. If the answer feels like "add a table," reconsider — it's static.
- Stored-procedure-only writes is the project rule for *data* projects (AccessManager, Connect). This site has no writes worth speaking of, so the rule is N/A here.

---

## 19. Definition of Done — v0.1 scaffold

After the 2-day sprint, the following must be true:

- [ ] Solution builds clean, no warnings as errors above level 3
- [ ] `dotnet run` serves `/` with a real home page using approved content
- [ ] All routes in §7 resolve to a real page (no 404s on v1 inventory)
- [ ] Cookie consent banner shows on first visit, persists choice, gates analytics placeholder
- [ ] Footer has ZA contact details (even if some placeholders), all four legal page links resolve
- [ ] `lang="en-ZA"`, hreflang, `og:locale` correct on every page
- [ ] No copy on any page that isn't either (a) from the approved partner kit, (b) original ZA-team-authored, or (c) clearly marked `PLACEHOLDER`
- [ ] Repo pushed to `neaMetricsv2/suprema`, tagged `v0.1-scaffold`
- [ ] README explains how to run locally and points at this plan

Phase 4 starts after v0.1 is in Tshepo's hands.

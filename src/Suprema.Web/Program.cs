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

// Localization plumbing — en-ZA only for v1, ready for af-ZA / zu-ZA
var supportedCultures = new[] { new CultureInfo("en-ZA") };
builder.Services.Configure<RequestLocalizationOptions>(o =>
{
    o.DefaultRequestCulture = new RequestCulture("en-ZA");
    o.SupportedCultures = supportedCultures;
    o.SupportedUICultures = supportedCultures;
});

var app = builder.Build();

// Eagerly initialise ContentService so startup counts are logged immediately
_ = app.Services.GetRequiredService<Suprema.Core.Abstractions.IContentService>();

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

// Named static pages: /about, /contact, /news  (clean URLs per §7 routing map)
app.MapControllerRoute(
    name: "about",
    pattern: "about",
    defaults: new { controller = "Pages", action = "Show", slug = "about" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

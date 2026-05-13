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

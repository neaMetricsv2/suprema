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

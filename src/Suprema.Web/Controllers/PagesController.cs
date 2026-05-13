using Microsoft.AspNetCore.Mvc;
using Suprema.Core.Abstractions;
using Suprema.Web.ViewModels;

namespace Suprema.Web.Controllers;

public sealed class PagesController(IContentService content) : Controller
{
    public IActionResult Show(string slug)
    {
        var page = content.GetPage(slug);
        return page is null ? NotFound() : View(new ContentPageViewModel { Page = page });
    }
}

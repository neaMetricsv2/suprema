using Microsoft.AspNetCore.Mvc;
using Suprema.Core.Abstractions;

namespace Suprema.Web.Controllers;

public sealed class NewsController(IContentService content) : Controller
{
    public IActionResult Index() => View(content.GetArticles());

    public IActionResult Article(string slug)
    {
        var article = content.GetArticle(slug);
        return article is null ? NotFound() : View(article);
    }
}

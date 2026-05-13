using Microsoft.AspNetCore.Mvc;
using Suprema.Core.Abstractions;

namespace Suprema.Web.Controllers;

public sealed class SolutionsController(IContentService content) : Controller
{
    public IActionResult Index() => View(content.GetSolutions());

    public IActionResult Detail(string slug)
    {
        var solution = content.GetSolution(slug);
        return solution is null ? NotFound() : View(solution);
    }
}

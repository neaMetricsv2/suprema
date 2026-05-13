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

        // TODO: route to CRM / mailbox in Phase 4
        return RedirectToAction(nameof(ThankYou));
    }

    public IActionResult ThankYou() => View();
}

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ClickAndGoApp.ViewModels;

namespace ClickAndGoApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> logger;

    public HomeController(ILogger<HomeController> logger)
    {
        this.logger = logger;
    }

    public IActionResult Index()
    {
        // Show the welcome message only on the very first visit of the session.
        // "welcomed" is a flag stored in session — once set, the message is never shown again
        // until the user logs out and starts a new session.
        if (HttpContext.Session.GetInt32("welcomed") == null)
        {
            HttpContext.Session.SetInt32("welcomed", 1);
            // Don't overwrite a success message that may have been set by a redirect (e.g. after login).
            if (TempData["Success"] == null)
                TempData["Success"] = "Bienvenue sur notre site";
        }

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel(Activity.Current?.Id ?? HttpContext.TraceIdentifier));
    }
}

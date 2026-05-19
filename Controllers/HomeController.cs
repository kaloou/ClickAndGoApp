using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ClickAndGoApp.DAL;
using ClickAndGoApp.Models;
using ClickAndGoApp.ViewModels;

namespace ClickAndGoApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> logger;
    private readonly IRecipeDAL recipeDal;

    public HomeController(ILogger<HomeController> logger, IRecipeDAL recipeDal)
    {
        this.logger    = logger;
        this.recipeDal = recipeDal;
    }

    public async Task<IActionResult> Index()
    {
        if (HttpContext.Session.GetInt32("welcomed") == null)
        {
            TempData["Success"] = "Bienvenue sur notre site";
            HttpContext.Session.SetInt32("welcomed", 1);
        }

        List<Recipe> recipes = await Recipe.GetAllAsync(recipeDal);
        return View(recipes.Take(4).ToList());
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel(Activity.Current?.Id ?? HttpContext.TraceIdentifier));
    }
}
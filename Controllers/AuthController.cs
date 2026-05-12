using ClickAndGoApp.DAL;
using ClickAndGoApp.Models;
using Microsoft.AspNetCore.Mvc;
namespace ClickAndGoApp.Controllers;

public class AuthController : Controller
{
    private readonly UserDAL _userDal;

    public AuthController(UserDAL userDAL)
    {
        _userDal = userDAL;
    }
    
    [HttpGet]//Quand le user arrive y'a juste le formulaire
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]//Redirige selon le role
    public IActionResult Login(string email, string password)
    {
        User user = _userDal.GetByCredentials(email, password);
        if (user == null)
        {
            ViewBag.Error = "Invalid email or password";
            return View();
        }

        HttpContext.Session.SetInt32("userId", user.UserId);
        HttpContext.Session.SetString("role", user.Role);
        HttpContext.Session.SetString("firstName", user.FirstName);

        if (user.Role == "OrderPicker")
            return RedirectToAction("Index", "OrderPicker"); //
        else if (user.Role == "Cashier")
            return RedirectToAction("Index", "Cashier");
        else
            return RedirectToAction("Login","Product");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
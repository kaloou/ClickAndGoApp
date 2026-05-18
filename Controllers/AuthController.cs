using Microsoft.AspNetCore.Mvc;
using ClickAndGoApp.DAL;
using ClickAndGoApp.Models;
using ClickAndGoApp.DAL.interfaces;

namespace ClickAndGoApp.Controllers;

public class AuthController : Controller
{
    private readonly UserDAL     userDal;
    private readonly ICustomerDAL customerDal;

    public AuthController(UserDAL userDal, ICustomerDAL customerDal)
    {
        this.userDal     = userDal;
        this.customerDal = customerDal;
    }
    
    // ====== Login page ======
    [HttpGet] 
    public IActionResult Login()
    {
        return View();
    }
    
    // ====== Register page ======
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    // ====== Login ====== (Redirige selon le role)
    [HttpPost]
    public async Task<IActionResult> LoginAsync(string email, string password)
    {
        var user = await Models.User.GetByCredentialsAsync(email, password, userDal);
        if (user == null)
        {
            ViewBag.Error = "Invalid email or password";
            return View();
        }
        
        // Données de session 
        HttpContext.Session.SetInt32("userId", user.UserId);
        HttpContext.Session.SetString("role", user.Role);
        HttpContext.Session.SetString("firstName", user.FirstName);

        if (user.Role == "OrderPicker")
            return RedirectToAction("Index", "OrderPicker");
        else if (user.Role == "Cashier")
            return RedirectToAction("Index", "Cashier");
        else
            return Redirect($"/");
    }
    
    [HttpPost]
    public async Task<IActionResult> Register(string firstName, string lastName, string email, string password, string? phoneNumber, string? address)
    {
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
            string.IsNullOrWhiteSpace(email)     || string.IsNullOrWhiteSpace(password))
        {
            ViewBag.Error = "Invalid information";
            return View();
        }

        bool emailExists = await Customer.GetByEmailAsync(email, customerDal);
        if (emailExists)
        {
            ViewBag.error = "email already used";
            return View();
        }
        
        await Customer.CreateAccountAsync(firstName, lastName, email, password, phoneNumber, address, customerDal);
        TempData["Success"] = "Account created";
        return RedirectToAction("Login");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
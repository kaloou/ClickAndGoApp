using Microsoft.AspNetCore.Mvc;
using ClickAndGoApp.DAL;
using ClickAndGoApp.Models;
using ClickAndGoApp.ViewModels;

namespace ClickAndGoApp.Controllers;

public class AuthController : Controller
{
    private readonly IUserDAL     userDal;
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
        return View(new LoginViewModel());
    }
    // ====== Login ====== (Redirige selon le role)
    [HttpPost]
    public async Task<IActionResult> LoginAsync(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        Models.User user = await Models.User.GetByCredentialsAsync(model.Email, model.Password, userDal);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password");
            return View(model);
        }

        CreateSession(user);

        if (user.Role == "OrderPicker")
            return RedirectToAction("Index", "OrderPicker");
        else if (user.Role == "Cashier")
            return RedirectToAction("Index", "Cashier");
        else
            return Redirect($"/");
    }
    
    private void CreateSession(Models.User user)
    {
        HttpContext.Session.SetInt32("userId", user.UserId);
        HttpContext.Session.SetString("role", user.Role);
        HttpContext.Session.SetString("firstName", user.FirstName);
    }
    
    // ====== Register page ======
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    
    // Create Account
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

    // Log out
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}

using Microsoft.AspNetCore.Mvc;
using ClickAndGoApp.DAL;
using ClickAndGoApp.Models;
using ClickAndGoApp.ViewModels;

namespace ClickAndGoApp.Controllers;

public class AuthController : Controller
{
    private readonly IUserDAL      userDal;
    private readonly ICustomerDAL  customerDal;
    private readonly IOrderDAL     orderDal;
    private readonly IOrderLineDAL orderLineDal;

    public AuthController(UserDAL userDal, ICustomerDAL customerDal, IOrderDAL orderDal, IOrderLineDAL orderLineDal)
    {
        this.userDal     = userDal;
        this.customerDal = customerDal;
        this.orderDal    = orderDal;
        this.orderLineDal = orderLineDal;
    }
    
    // ====== Login page ======
    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }
    // ====== Login ====== (Redirige selon le role)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LoginAsync(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        Models.User user = await Models.User.GetByCredentialsAsync(model.Email, model.Password, userDal);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Email ou mot de passe incorrect.");
            return View(model);
        }

        CreateSession(user);

        if (user.Role == "Customer")
        {
            int? existingCartId = await orderDal.GetActiveCartAsync(user.UserId);
            if (existingCartId.HasValue)
            {
                HttpContext.Session.SetInt32("orderId", existingCartId.Value);
                List<OrderLine> lines = await orderLineDal.GetOrderLinesAsync(existingCartId.Value);
                HttpContext.Session.SetInt32("cartCount", lines.Count);
            }
            return Redirect("/");
        }

        if (user.Role == "OrderPicker")
            return RedirectToAction("Index", "OrderPicker");

        return RedirectToAction("Index", "Cashier");
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
    [ValidateAntiForgeryToken]
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
        TempData["Success"] = "Vous avez été déconnecté.";
        return RedirectToAction("Index", "Home");
    }
}
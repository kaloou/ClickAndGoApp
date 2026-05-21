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
    private readonly ITimeSlotDAL  timeSlotDal;

    public AuthController(UserDAL userDal, ICustomerDAL customerDal, IOrderDAL orderDal, IOrderLineDAL orderLineDal, ITimeSlotDAL timeSlotDal)
    {
        this.userDal      = userDal;
        this.customerDal  = customerDal;
        this.orderDal     = orderDal;
        this.orderLineDal = orderLineDal;
        this.timeSlotDal  = timeSlotDal;
    }

    // ====== Login page ======
    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    // ====== Login POST ======
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
            // Restore the cart from the DB so the session reflects the customer's existing basket.
            int? existingCartId = await orderDal.GetActiveCartAsync(user.UserId);
            if (existingCartId.HasValue)
            {
                HttpContext.Session.SetInt32("orderId", existingCartId.Value);
                List<OrderLine> lines = await orderLineDal.GetOrderLinesAsync(existingCartId.Value);
                HttpContext.Session.SetInt32("cartCount", lines.Count);

                // Also restore the selected store if the cart already has a time slot.
                Order cart = await Order.GetByIdAsync(existingCartId.Value, orderDal);
                if (cart.TimeSlotId != 0)
                {
                    int? storeId = await timeSlotDal.GetStoreIdAsync(cart.TimeSlotId);
                    if (storeId.HasValue)
                        HttpContext.Session.SetInt32("selectedStoreId", storeId.Value);
                }
            }

            // If the user tried to add a product or recipe before logging in,
            // redirect them to the handler that will complete the add-to-cart action.
            if (HttpContext.Session.GetInt32("pendingProductId").HasValue)
                return RedirectToAction("HandlePendingProduct", "Product");

            if (HttpContext.Session.GetInt32("pendingRecipeId").HasValue)
                return RedirectToAction("HandlePendingIngredients", "Recipe");

            return Redirect("/");
        }

        if (user.Role == "OrderPicker")
            return RedirectToAction("Index", "OrderPicker");

        return RedirectToAction("Index", "Cashier");
    }

    // Stores the minimum user info in session — enough to identify the user and control access.
    // We avoid storing the full User object to keep session data small.
    private void CreateSession(Models.User user)
    {
        HttpContext.Session.SetInt32("userId",    user.UserId);
        HttpContext.Session.SetString("role",      user.Role);
        HttpContext.Session.SetString("firstName", user.FirstName);
    }

    // ====== Register page ======
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    // ====== Register POST ======
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

        // Check for duplicate email before attempting to insert.
        bool emailExists = await Customer.GetByEmailAsync(email, customerDal);
        if (emailExists)
        {
            ViewBag.error = "email already used";
            return View();
        }

        Customer newCustomer = await Customer.CreateAccountAsync(firstName, lastName, email, password, phoneNumber, address, customerDal);
        // Log the customer in immediately after registration so they don't have to log in again.
        CreateSession(newCustomer);
        TempData["Success"] = "Account created";
        return Redirect("/");
    }

    // ====== Logout ======
    public IActionResult Logout()
    {
        // Clear wipes all session data — cart, role, userId, everything.
        HttpContext.Session.Clear();
        TempData["Success"] = "Vous avez été déconnecté.";
        return RedirectToAction("Index", "Home");
    }
}

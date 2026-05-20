using Microsoft.AspNetCore.Mvc;
using ClickAndGoApp.DAL;
using ClickAndGoApp.Exceptions;
using ClickAndGoApp.Models;
using ClickAndGoApp.ViewModels;

namespace ClickAndGoApp.Controllers;

public class CustomerController : Controller
{
    private readonly ICustomerDAL customerDal;
    private readonly IOrderDAL    orderDal;

    public CustomerController(ICustomerDAL customerDal, IOrderDAL orderDal)
    {
        this.customerDal = customerDal;
        this.orderDal    = orderDal;
    }

    // ====== Profile ======
    public async Task<IActionResult> Profile()
    {
        int? userId = HttpContext.Session.GetInt32("userId");
        if (userId == null || HttpContext.Session.GetString("role") != "Customer")
            return RedirectToAction("Login", "Auth");

        try
        {
            Customer customer   = await Customer.GetByIdAsync(userId.Value, customerDal);
            List<Order> orders  = await Order.GetOrdersByCustomerAsync(userId.Value, orderDal);
            return View(new CustomerAccountViewModel(customer, orders));
        }
        catch (EntityNotFoundException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // ====== Update Profile ======
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(string firstName, string lastName, int phoneNumber, string? address, string password)
    {
        int? userId = HttpContext.Session.GetInt32("userId");
        if (userId == null)
            return RedirectToAction("Login", "Auth");

        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(password))
        {
            TempData["Error"] = "Invalid data";
            return RedirectToAction("Profile");
        }

        try
        {
            Customer customer = await Customer.GetByIdAsync(userId.Value, customerDal);
            await customer.UpdateAsync(firstName, lastName, phoneNumber, address, password, customerDal);

            HttpContext.Session.SetString("firstName", firstName);
            TempData["Success"] = "Profile updated";
        }
        catch (EntityNotFoundException)
        {
            TempData["Error"] = "Customer not found";
        }

        return RedirectToAction("Profile");
    }
}

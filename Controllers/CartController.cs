using Microsoft.AspNetCore.Mvc;
using ClickAndGoApp.DAL;
using ClickAndGoApp.Models;
using ClickAndGoApp.ViewModels;

namespace ClickAndGoApp.Controllers;

public class CartController : Controller
{
    private readonly IOrderDAL     orderDal;
    private readonly IOrderLineDAL orderLineDal;

    public CartController(IOrderDAL orderDal, IOrderLineDAL orderLineDal)
    {
        this.orderDal     = orderDal;
        this.orderLineDal = orderLineDal;
    }

    // ====== Manage Cart ======
    public async Task<IActionResult> Index()
    {
        int? orderId = HttpContext.Session.GetInt32("orderId");
        // No orderId in session means the user has no active cart — show an empty cart view.
        if (orderId == null)
            return View(new CartViewModel(null, new()));

        Order order = await Order.GetByIdAsync(orderId.Value, orderDal);
        if (order == null)
            return View(new CartViewModel(null, new()));

        // on récupère sa liste de produits (peuplée dans order.OrderLines)
        await order.GetOrderLinesAsync(orderLineDal);

        // sync du badge panier
        HttpContext.Session.SetInt32("cartCount", order.OrderLines.Count);

        return View(new CartViewModel(order, order.OrderLines));
    }

    // ====== Remove Product ======
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveProduct(int productId)
    {
        int? orderId = HttpContext.Session.GetInt32("orderId");
        if (orderId == null)
            return RedirectToAction("Index");

        Order order = await Order.GetByIdAsync(orderId.Value, orderDal);
        await order.GetOrderLinesAsync(orderLineDal);

        // Find the matching order line — if it exists, remove it and decrement the badge count.
        OrderLine existing = order.GetOrderLine(productId);
        if (existing != null)
        {
            order.RemoveOrderLine(existing);
            await existing.RemoveAsync(order.OrderId, orderLineDal);
            int count = HttpContext.Session.GetInt32("cartCount") ?? 0;
            HttpContext.Session.SetInt32("cartCount", Math.Max(0, count - 1)); // prevent going below 0
        }

        TempData["Success"] = "Produit retiré du panier";
        return RedirectToAction("Index");
    }

    // ====== Update Quantity ======
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
    {
        int? orderId = HttpContext.Session.GetInt32("orderId");
        if (orderId == null)
            return RedirectToAction("Index");

        Order order = await Order.GetByIdAsync(orderId.Value, orderDal);
        await order.GetOrderLinesAsync(orderLineDal);

        // Quantity is capped at 10 to prevent unreasonable orders.
        if (quantity > 0 && quantity <= 10)
        {
            // on check bien si le produit choisi est dans la liste de produits
            OrderLine existing = order.GetOrderLine(productId);
            if (existing != null)
                await existing.SetQuantityAsync(order.OrderId, quantity, orderLineDal);

            TempData["Success"] = "Quantité mise à jour";
            return RedirectToAction("Index");
        }
        else
        {
            TempData["Error"] = "Quantité invalide";
            return RedirectToAction("Index");
        }
    }
}

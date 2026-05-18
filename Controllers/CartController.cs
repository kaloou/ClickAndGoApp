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
        // on check si une order(Cart) est bien en cours, sinon on envoie le view model avec données vides
        int? orderId = HttpContext.Session.GetInt32("orderId");
        if (orderId == null)
            return View(new CartViewModel { Order = null, OrderLines = new() });

        // on récupère order(cart)
        Order order = await Order.GetByIdAsync(orderId.Value, orderDal);
        if (order == null) // order id corrompu
            return View(new CartViewModel { Order = null, OrderLines = new() });

        // on récupère sa liste de produits
        List<OrderLine> orderLines = await order.GetOrderLinesAsync(orderLineDal);

        return View(new CartViewModel { Order = order, OrderLines = orderLines });
    }

    // ====== Remove Product ======
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveProduct(int productId)
    {
        // on check si l'order(cart) existe
        int? orderId = HttpContext.Session.GetInt32("orderId");
        if (orderId == null)
            return RedirectToAction("Index");

        //on l'a récupère + sa liste de produits
        Order order = await Order.GetByIdAsync(orderId.Value, orderDal);
        List<OrderLine> orderLines = await order.GetOrderLinesAsync(orderLineDal);

        // on check bien si le produit choisi est dans la liste de produits
        OrderLine existing = orderLines.FirstOrDefault(ol => ol.Product.ProductId == productId);
        if (existing != null)
            await existing.RemoveAsync(orderLineDal);

        TempData["Success"] = "Produit retiré du panier";
        return RedirectToAction("Index");
    }

    // ====== Update Quantity ======
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
    {
        // on check si l'order(cart) existe
        int? orderId = HttpContext.Session.GetInt32("orderId");
        if (orderId == null)
            return RedirectToAction("Index");

        //on l'a récupère + sa liste de produits
        Order order = await Order.GetByIdAsync(orderId.Value, orderDal);
        List<OrderLine> orderLines = await order.GetOrderLinesAsync(orderLineDal);

        if (quantity > 0) 
        {
            // on check bien si le produit choisi est dans la liste de produits
            OrderLine existing = orderLines.FirstOrDefault(ol => ol.Product.ProductId == productId);
            if (existing != null)
                await existing.SetQuantityAsync(quantity, orderLineDal);
            
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

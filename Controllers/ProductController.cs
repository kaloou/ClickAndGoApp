using Microsoft.AspNetCore.Mvc;
using ClickAndGoApp.DAL;
using ClickAndGoApp.Models;
using ClickAndGoApp.ViewModels;

namespace ClickAndGoApp.Controllers;

public class ProductController : Controller
{
    private readonly IProductDAL productDal;
    private readonly ICategoryDAL categoryDal;
    private readonly IOrderDAL orderDal;
    private readonly IOrderLineDAL orderLineDal;

    public ProductController(IProductDAL productDal, ICategoryDAL categoryDal,
        IOrderDAL orderDal, IOrderLineDAL orderLineDal)
    {
        this.productDal   = productDal;
        this.categoryDal  = categoryDal;
        this.orderDal     = orderDal;
        this.orderLineDal = orderLineDal;
    }

    // ====== Browse Products ======
    // ===== FilterPerCategory ===== extend
    public async Task<IActionResult> BrowseProductsAsync(int? categoryId)
    {
        List<Product> products;
        List<Category> categories = new();
        try
        {
            // On récupère les produits de base OU les produits filtrés par la catégorie choisie
            if (categoryId.HasValue)
                products = await Category.GetByCategoryAsync(categoryId.Value, productDal);
            else
                products = await Product.GetAllAsync(productDal);

            categories = await Category.GetAllAsync(categoryDal);
        }
        catch (Exception)
        {
            TempData["Error"] = "Impossible de charger les produits ou catégories";
            products = new();
        }

        var vm = new BrowseViewModel(products, categories, categoryId);

        return View(vm);
    }
    
    // ====== Add Product To Cart ======
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddProductToCart(int productId, int quantity = 1)
    {
        // On vérifie si l'utilisateur est bien connecté
        int? userId = HttpContext.Session.GetInt32("userId");
        if (userId is null)
        {
            HttpContext.Session.SetInt32("pendingProductId", productId);
            HttpContext.Session.SetInt32("pendingQuantity", quantity);
            return RedirectToAction("Login", "Auth");
        }

        // On récupère l'id de l'Order(Cart), si pas présent, on cherche en DB avant de créer
        int? orderId = HttpContext.Session.GetInt32("orderId");
        if (orderId is null)
        {
            int? existingId = await orderDal.GetActiveCartAsync(userId.Value);
            int newOrderId  = existingId ?? await orderDal.CreateOrderAsync(userId.Value);
            HttpContext.Session.SetInt32("orderId", newOrderId);
            orderId = newOrderId;
        }

        Order order = await Order.GetByIdAsync(orderId.Value, orderDal);
        if (order is null)
            return RedirectToAction("BrowseProducts");

        List<OrderLine> orderLines = await order.GetOrderLinesAsync(orderLineDal); // Panier

        // Null si le produit n'est pas encore dans le panier, un OrderLine (produit) s'il est déja dedans
        OrderLine existing = orderLines.FirstOrDefault(ol => ol.Product.ProductId == productId);
        
        if (existing == null)
        {
            await order.AddProductAsync(productId, orderLineDal, quantity);
            int count = HttpContext.Session.GetInt32("cartCount") ?? 0;
            HttpContext.Session.SetInt32("cartCount", count + 1);
        }
        else
        {
            await existing.SetQuantityAsync(existing.Quantity + quantity, orderLineDal);
        }

        await HttpContext.Session.CommitAsync();
        TempData["Success"] = "Produit ajouté au panier";
        return RedirectToAction("BrowseProducts");
    }
    
    // ====== Select Product ======
    public async Task<IActionResult> SelectProduct(int productId)
    {
        Product product = await Product.GetByIdAsync(productId, productDal);
        if (product == null)
            return RedirectToAction("BrowseProducts");
        return View(product);
    }

    // ====== Handle Pending Product (after login) ======
    public async Task<IActionResult> HandlePendingProduct()
    {
        int? userId    = HttpContext.Session.GetInt32("userId");
        int? productId = HttpContext.Session.GetInt32("pendingProductId");
        int  quantity  = HttpContext.Session.GetInt32("pendingQuantity") ?? 1;

        if (userId == null || !productId.HasValue)
            return RedirectToAction("BrowseProducts");

        int? orderId = HttpContext.Session.GetInt32("orderId");
        if (orderId == null)
        {
            int? existingId = await orderDal.GetActiveCartAsync(userId.Value);
            int newOrderId  = existingId ?? await orderDal.CreateOrderAsync(userId.Value);
            HttpContext.Session.SetInt32("orderId", newOrderId);
            orderId = newOrderId;
        }

        Order order = await Order.GetByIdAsync(orderId.Value, orderDal);
        List<OrderLine> orderLines = await order.GetOrderLinesAsync(orderLineDal);

        OrderLine existing = orderLines.FirstOrDefault(ol => ol.Product.ProductId == productId.Value);
        if (existing == null)
        {
            await order.AddProductAsync(productId.Value, orderLineDal, quantity);
            int count = HttpContext.Session.GetInt32("cartCount") ?? 0;
            HttpContext.Session.SetInt32("cartCount", count + 1);
        }
        else
            await existing.SetQuantityAsync(existing.Quantity + quantity, orderLineDal);

        HttpContext.Session.Remove("pendingProductId");
        HttpContext.Session.Remove("pendingQuantity");

        TempData["Success"] = "Produit ajouté au panier";
        return RedirectToAction("BrowseProducts");
    }
}

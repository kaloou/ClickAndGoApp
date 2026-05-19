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
            return RedirectToAction("Login", "Auth");

        // On récupère l'id de l'Order(Cart), si pas présent, on le crée au vol
        int? orderId = HttpContext.Session.GetInt32("orderId");
        if (orderId is null)
        {
            int newOrderId = await orderDal.CreateOrderAsync(userId.Value);
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
            await order.AddProductAsync(productId, orderLineDal, quantity);
        else
            await existing.SetQuantityAsync(existing.Quantity + quantity, orderLineDal);

        TempData["Success"] = "Produit ajouté au panier";
        return RedirectToAction("SelectProduct", new { productId = productId });
    }
    
    // ====== Select Product ======
    public async Task<IActionResult> SelectProduct(int productId)
    {
        Product product = await Product.GetByIdAsync(productId, productDal);
        if (product == null) 
            return RedirectToAction("BrowseProducts");
        return View(product);
    }
}

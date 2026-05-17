using Microsoft.AspNetCore.Mvc;
using ClickAndGoApp.DAL.interfaces;
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
    // FilterPerCategory extends ici : même action, categoryId déclenche le filtre
    public async Task<IActionResult> BrowseProductsAsync(int? categoryId)
    {
        List<Product> products;
        List<Category> categories = new();
        try
        {
            if (categoryId.HasValue)
                products = await Category.GetByCategory(categoryId.Value, productDal);
            else
                products = await Product.GetAll(productDal);
            
            categories = await Category.GetAll(categoryDal);
        }
        catch (Exception)
        {                        
            TempData["Error"] = "Impossible de charger les produits ou catégories";
            products = new();
        }
        
        var vm = new BrowseViewModel
        {
            Products           = products,
            Categories         = categories,
            SelectedCategoryId = categoryId
        };

        return View(vm);
    }

    // ====== Select Product ======
    public async Task<IActionResult> SelectProduct(int productId)
    {
        Product product = await Product.GetById(productId, productDal);
        if (product == null) 
            return RedirectToAction("BrowseProducts");
        return View(product);
    }

    // ====== Add Product To Cart ======
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddProductToCart(int productId, int quantity = 1)
    {
        int? userId = HttpContext.Session.GetInt32("userId");
        if (userId == null) // On redirect au login si pas connecté maintenant plus d'offline Cart
            return RedirectToAction("Login", "Auth");

        int? orderId = HttpContext.Session.GetInt32("orderId");
        if (orderId == null)
        {
            int newOrderId = await orderDal.CreateOrder(userId.Value);
            HttpContext.Session.SetInt32("orderId", newOrderId);
            orderId = newOrderId;
        }

        Order order = await Order.GetById(orderId.Value, orderDal);
        if (order == null)
            return RedirectToAction("BrowseProducts");

        List<OrderLine> orderLines = await order.GetOrderLines(orderLineDal); // Panier

        OrderLine existing = orderLines.FirstOrDefault(ol => ol.Product.ProductId == productId);
        // Null si le produit n'est pas encore dans le panier, un OrderLine (produit) si il est déja dedans
        if (existing == null)
            await order.AddProduct(productId, orderLineDal, quantity);
        else
            await existing.SetQuantity(productId, existing.Quantity + quantity, orderLineDal);

        TempData["Success"] = "Produit ajouté au panier";
        return RedirectToAction("SelectProduct", new { productId = productId });
    }
}

using Microsoft.AspNetCore.Mvc;
using ClickAndGoApp.DAL;
using ClickAndGoApp.Models;
using ClickAndGoApp.Exceptions;

namespace ClickAndGoApp.Controllers;

public class RecipeController : Controller
{
    private readonly IRecipeDAL recipeDal;
    private readonly IRecipeIngredientDAL recipeIngredientDal;
    private readonly IOrderDAL orderDal;
    private readonly IOrderLineDAL orderLineDal;

    public RecipeController(IRecipeDAL recipeDal, IRecipeIngredientDAL recipeIngredientDal,
        IOrderDAL orderDal, IOrderLineDAL orderLineDal)
    {
        this.recipeDal           = recipeDal;
        this.recipeIngredientDal = recipeIngredientDal;
        this.orderDal            = orderDal;
        this.orderLineDal        = orderLineDal;
    }

    // ====== BrowseRecipes ======
    public async Task<IActionResult> Index()
    {
        List<Recipe> recipes = await Recipe.GetAllAsync(recipeDal);
        return View(recipes);
    }

    // ====== SelectRecipe ======
    public async Task<IActionResult> SelectRecipe(int recipeId)
    {
        try
        {
            Recipe recipe = await Recipe.GetByIdAsync(recipeId, recipeDal);
            return View(recipe);
        }
        catch (EntityNotFoundException)
        {
            return RedirectToAction("Index");
        }
    }

    // ====== Pending Add Ingredients (non connecté → login) ======
    public IActionResult PendingAddIngredients(int recipeId)
    {
        HttpContext.Session.SetInt32("pendingRecipeId", recipeId);
        return RedirectToAction("Login", "Auth");
    }

    // ====== Handle Pending Ingredients (après login) ======
    public async Task<IActionResult> HandlePendingIngredients()
    {
        int? userId   = HttpContext.Session.GetInt32("userId");
        int? recipeId = HttpContext.Session.GetInt32("pendingRecipeId");

        if (userId == null || !recipeId.HasValue)
            return RedirectToAction("Index");

        try
        {
            using (Recipe recipe = await Recipe.GetByIdAsync(recipeId.Value, recipeDal))
            {
                int? orderId = HttpContext.Session.GetInt32("orderId");
                if (orderId == null)
                {
                    int? existingId = await orderDal.GetActiveCartAsync(userId.Value);
                    int newOrderId  = existingId ?? await orderDal.CreateOrderAsync(userId.Value);
                    HttpContext.Session.SetInt32("orderId", newOrderId);
                    orderId = newOrderId;
                }

                using (Order order = await Order.GetByIdAsync(orderId.Value, orderDal))
                {
                    List<RecipeIngredient> ingredients = await recipe.GetIngredientsAsync(recipeIngredientDal);

                    if (ingredients.Count > 0)
                    {
                        List<OrderLine> orderLines = await order.GetOrderLinesAsync(orderLineDal);
                        int newProducts = 0;
                        foreach (RecipeIngredient ingredient in ingredients)
                        {
                            OrderLine existing = orderLines.FirstOrDefault(ol => ol.Product.ProductId == ingredient.ProductId);
                            if (existing == null)
                            {
                                await order.AddProductAsync(ingredient.ProductId, orderLineDal, ingredient.Quantity);
                                newProducts++;
                            }
                            else
                                await existing.SetQuantityAsync(existing.Quantity + ingredient.Quantity, orderLineDal);
                        }
                        int count = HttpContext.Session.GetInt32("cartCount") ?? 0;
                        HttpContext.Session.SetInt32("cartCount", count + newProducts);
                    }
                }
            }
        }
        catch (EntityNotFoundException)
        {
            return RedirectToAction("Index");
        }

        HttpContext.Session.Remove("pendingRecipeId");
        TempData["Success"] = "Ingrédients ajoutés au panier";
        return RedirectToAction("Index");
    }

    // ====== AddIngredientsToCart ======
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddIngredientsToCart(int recipeId)
    {
        int? userId = HttpContext.Session.GetInt32("userId");
        if (userId is null)
            return RedirectToAction("Login", "Auth");

        try
        {
            // 2-3 : GetById(recipeId) → recipe
            using (Recipe recipe = await Recipe.GetByIdAsync(recipeId, recipeDal))
            {
                int? orderId = HttpContext.Session.GetInt32("orderId");
                if (orderId is null)
                {
                    int? existingId = await orderDal.GetActiveCartAsync(userId.Value);
                    int newOrderId  = existingId ?? await orderDal.CreateOrderAsync(userId.Value);
                    HttpContext.Session.SetInt32("orderId", newOrderId);
                    orderId = newOrderId;
                }

                // 4-5 : GetById(orderId) → order
                using (Order order = await Order.GetByIdAsync(orderId.Value, orderDal))
                {
                    // 6-8 : GetIngredients() → ingredients
                    List<RecipeIngredient> ingredients = await recipe.GetIngredientsAsync(recipeIngredientDal);

                    // alt [all ingredients are available]
                    if (ingredients.Count > 0)
                    {
                        List<OrderLine> orderLines = await order.GetOrderLinesAsync(orderLineDal);

                        int newProducts = 0;
                        foreach (RecipeIngredient ingredient in ingredients)
                        {
                            OrderLine existing = orderLines.FirstOrDefault(ol => ol.Product.ProductId == ingredient.ProductId);
                            if (existing == null)
                            {
                                await order.AddProductAsync(ingredient.ProductId, orderLineDal, ingredient.Quantity);
                                newProducts++;
                            }
                            else
                                await existing.SetQuantityAsync(existing.Quantity + ingredient.Quantity, orderLineDal);
                        }

                        int count = HttpContext.Session.GetInt32("cartCount") ?? 0;
                        HttpContext.Session.SetInt32("cartCount", count + newProducts);

                        await HttpContext.Session.CommitAsync();
                        TempData["Success"] = "Ingrédients ajoutés au panier";
                        return RedirectToAction("SelectRecipe", new { recipeId });
                    }
                    else
                    {
                        // 12 : no ingredients added to cart
                        TempData["Error"] = "Aucun ingrédient disponible pour cette recette";
                        return RedirectToAction("SelectRecipe", new { recipeId });
                    }
                }
            }
        }
        catch (EntityNotFoundException)
        {
            return RedirectToAction("Index");
        }
    }
}

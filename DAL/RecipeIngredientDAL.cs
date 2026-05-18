using ClickAndGoApp.Models;

namespace ClickAndGoApp.DAL;

public class RecipeIngredientDAL : IRecipeIngredientDAL
{
    private readonly DBConnection db;

    public RecipeIngredientDAL(DBConnection db)
    {
        this.db = db;
    }

    public Task<List<RecipeIngredient>> GetByRecipeAsync(int recipeId) => Task.FromResult<List<RecipeIngredient>>(null);
}

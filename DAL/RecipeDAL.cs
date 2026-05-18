using ClickAndGoApp.Models;

namespace ClickAndGoApp.DAL;

public class RecipeDAL : IRecipeDAL
{
    private readonly DBConnection db;

    public RecipeDAL(DBConnection db)
    {
        this.db = db;
    }

    public Task<List<Recipe>> GetAllAsync() => Task.FromResult<List<Recipe>>(null);
    public Task<Recipe> GetByIdAsync(int recipeId) => Task.FromResult<Recipe>(null);
}

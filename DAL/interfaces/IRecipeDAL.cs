using ClickAndGoApp.Models;

namespace ClickAndGoApp.DAL;

public interface IRecipeDAL
{
    Task<List<Recipe>> GetAllAsync();
    Task<Recipe> GetByIdAsync(int recipeId);
}

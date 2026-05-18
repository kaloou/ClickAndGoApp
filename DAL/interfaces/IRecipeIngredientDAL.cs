using ClickAndGoApp.Models;

namespace ClickAndGoApp.DAL;

public interface IRecipeIngredientDAL
{
    Task<List<RecipeIngredient>> GetByRecipeAsync(int recipeId);
}

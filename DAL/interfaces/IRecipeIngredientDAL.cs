using ClickAndGoApp.Models;

namespace ClickAndGoApp.DAL.interfaces;

public interface IRecipeIngredientDAL
{
    Task<List<RecipeIngredient>> GetByRecipe(int recipeId);
}

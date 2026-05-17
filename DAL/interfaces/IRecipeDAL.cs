using ClickAndGoApp.Models;

namespace ClickAndGoApp.DAL.interfaces;

public interface IRecipeDAL
{
    Task<List<Recipe>> GetAll();
    Task<Recipe> GetById(int recipeId);
}

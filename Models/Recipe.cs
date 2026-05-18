using ClickAndGoApp.DAL.interfaces;

namespace ClickAndGoApp.Models;

public class Recipe
{
    private int recipeId;
    private string name;
    private string description;
    private List<RecipeIngredient> ingredients;

    public int RecipeId
    {
        get => recipeId;
        set => recipeId = value > 0
            ? value
            : throw new ArgumentException("RecipeId must be positive");
    }

    public string Name
    {
        get => name;
        set => name = !string.IsNullOrWhiteSpace(value)
            ? value
            : throw new ArgumentException("Name cannot be empty");
    }

    public string Description
    {
        get => description;
        set => description = value;
    }

    public List<RecipeIngredient> Ingredients
    {
        get => ingredients;
        set => ingredients = value;
    }

    public Recipe(int recipeId, string name, string description)
    {
        RecipeId    = recipeId;
        Name        = name;
        Description = description;
    }
    
    //Methodes====
    public static async Task<List<Recipe>> GetAllAsync(IRecipeDAL dal) => //
        await dal.GetAll();

    public static async Task<Recipe> GetByIdAsync(int recipeId, IRecipeDAL dal) => //
        await dal.GetById(recipeId);

    public async Task<List<RecipeIngredient>> GetIngredientsAsync(IRecipeIngredientDAL dal) => //
        await dal.GetByRecipe(recipeId);
}

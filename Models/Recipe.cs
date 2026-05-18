using ClickAndGoApp.DAL;

namespace ClickAndGoApp.Models;

public class Recipe
{
    private int recipeId;
    private string name;
    private string description;
    private LinkedList<RecipeIngredient> ingredients = new LinkedList<RecipeIngredient>();

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
        set => description = !string.IsNullOrWhiteSpace(value)
            ? value
            : throw new ArgumentException("Description cannot be empty");
    }

    public LinkedList<RecipeIngredient> Ingredients
    {
        get => ingredients;
        set => ingredients = value ?? throw new ArgumentNullException("Ingredients cannot be null");
    }

    public Recipe(int recipeId, string name, string description)
    {
        RecipeId    = recipeId;
        Name        = name;
        Description = description;
    }

    public static async Task<List<Recipe>> GetAllAsync(IRecipeDAL dal) =>
        await dal.GetAllAsync();

    public static async Task<Recipe> GetByIdAsync(int recipeId, IRecipeDAL dal) =>
        await dal.GetByIdAsync(recipeId);

    public async Task<List<RecipeIngredient>> GetIngredientsAsync(IRecipeIngredientDAL dal) =>
        await dal.GetByRecipeAsync(recipeId);

    public override string ToString() =>
        $"[Recipe] Id={RecipeId} | {Name}";

    public override bool Equals(object obj)
    {
        if (obj is not Recipe other) return false;
        return RecipeId == other.RecipeId;
    }

    public override int GetHashCode() => RecipeId.GetHashCode();
}

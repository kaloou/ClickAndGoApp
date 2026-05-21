using ClickAndGoApp.DAL;

namespace ClickAndGoApp.Models;

// IDisposable is implemented here for the same reason as Order
// a significant number of objects and we want to release them explicitly when done.
public class Recipe : IDisposable
{
    private int recipeId;
    private string name;
    private string description;
    // LinkedList is used because ingredients are iterated sequentially and never accessed by index.
    // It also makes AddLast() O(1), which is used when building the list in the DAL.
    private LinkedList<RecipeIngredient> ingredients = new LinkedList<RecipeIngredient>();
    private bool disposed = false;

    // ImagePath is optional — some recipes may not have a photo yet.
    public string? ImagePath { get; set; }

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

    // constructeur preview (GetAllAsync — pas d'ingrédients chargés)
    public Recipe(int recipeId, string name, string description)
    {
        RecipeId    = recipeId;
        Name        = name;
        Description = description;
    }

    // constructeur complet (composition 1..* — au moins 1 ingrédient obligatoire)
    public Recipe(int recipeId, string name, string description, RecipeIngredient firstIngredient)
        : this(recipeId, name, description)
    {
        if (firstIngredient == null)
            throw new ArgumentNullException(nameof(firstIngredient));
        firstIngredient.Recipe = this;
        ingredients.AddLast(firstIngredient);
    }

    // constructeur complet (composition 1..* — au moins 1 ingrédient obligatoire)
    public Recipe(int recipeId, string name, string description, RecipeIngredient firstIngredient)
        : this(recipeId, name, description)
    {
        if (firstIngredient == null)
            throw new ArgumentNullException(nameof(firstIngredient));
        firstIngredient.Recipe = this;
        ingredients.AddLast(firstIngredient);
    }

    public static async Task<List<Recipe>> GetAllAsync(IRecipeDAL dal)
        => await dal.GetAllAsync();

    public static async Task<Recipe> GetByIdAsync(int recipeId, IRecipeDAL dal)
        => await dal.GetByIdAsync(recipeId);

    // Ingredients are loaded on demand — we only need them on the detail page, not when listing recipes.
    public async Task<List<RecipeIngredient>> GetIngredientsAsync(IRecipeIngredientDAL dal)
    {
        var result = await dal.GetByRecipeAsync(recipeId);
        ingredients = new LinkedList<RecipeIngredient>(result);
        return result;
    }

    public void AddIngredient(RecipeIngredient ingredient)
    {
        if (ingredients.Any(i => i.ProductId == ingredient.ProductId))
            throw new InvalidOperationException($"Product {ingredient.ProductId} already in recipe");
        ingredient.Recipe = this;
        ingredients.AddLast(ingredient);
    }

    public void RemoveIngredient(RecipeIngredient ingredient)
    {
        ingredients.Remove(ingredient);
    }

    public RecipeIngredient? GetIngredient(int productId)
        => ingredients.FirstOrDefault(i => i.ProductId == productId);
    
    //==============================
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
                ingredients.Clear();
            disposed = true;
        }
    }

    ~Recipe() => Dispose(false);

    public override string ToString()
        => $"[Recipe] Id={RecipeId} | {Name}";

    public override bool Equals(object obj)
    {
        if (obj is not Recipe other) return false;
        return RecipeId == other.RecipeId;
    }

    public override int GetHashCode() => RecipeId.GetHashCode();
}

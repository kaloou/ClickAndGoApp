using ClickAndGoApp.DAL;

namespace ClickAndGoApp.Models;

public class Product
{
    private int productId;
    private string name;
    private float price;
    private Category category;
    private string imagePath;
    private string description;
    private HashSet<RecipeIngredient> recipeIngredients;

    public int ProductId
    {
        get => productId;
        set => productId = value > 0
            ? value
            : throw new ArgumentException("ProductId must be positive");
    }

    public string Name
    {
        get => name;
        set => name = !string.IsNullOrWhiteSpace(value)
            ? value
            : throw new ArgumentException("Name cannot be empty");
    }

    public float Price
    {
        get => price;
        set => price = value >= 0
            ? value
            : throw new ArgumentException("Price cannot be negative");
    }

    // Convenience shortcut so callers don't have to go through Category.
    public int CategoryId => category.CategoryId;

    public Category Category
    {
        get => category;
        set => category = value ?? throw new ArgumentNullException("Category cannot be null");
    }

    // Both ImagePath and Description are optional — not all products have them.
    public string ImagePath
    {
        get => imagePath;
        set => imagePath = value;
    }

    public string Description
    {
        get => description;
        set => description = value;
    }

    public HashSet<RecipeIngredient> RecipeIngredients
    {
        get => recipeIngredients;
        set => recipeIngredients = value;
    }

    public Product(int productId, string name, float price,
        Category category, string description, string imagePath = null)
    {
        ProductId         = productId;
        Name              = name;
        Price             = price;
        Category          = category;
        Description       = description;
        ImagePath         = imagePath;
        // HashSet ensures a product can only appear once per recipe (no duplicates), with O(1) Contains.
        recipeIngredients = new HashSet<RecipeIngredient>();
    }

    public static async Task<List<Product>> GetAllAsync(IProductDAL dal)
        => await dal.GetAllAsync();

    public static async Task<Product> GetByIdAsync(int productId, IProductDAL dal)
        => await dal.GetByIdAsync(productId);
}

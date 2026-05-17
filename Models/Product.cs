using ClickAndGoApp.DAL.interfaces;

namespace ClickAndGoApp.Models;

public class Product
{
    private int productId;
    private string name;
    private float price;
    private Category category;
    private string imagePath;
    private string description;
    private HashSet<RecipeIngredient> recipeIngredients = new HashSet<RecipeIngredient>();

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

    public Category Category
    {
        get => category;
        set => category = value ?? throw new ArgumentNullException("Category cannot be null");
    }

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
        ProductId   = productId;
        Name        = name;
        Price       = price;
        Category    = category;
        Description = description;
        ImagePath   = imagePath;
    }

    public static Task<List<Product>> GetAll(IProductDAL dal) =>
        dal.GetAll();

    public static Task<Product> GetById(int productId, IProductDAL dal) =>
        dal.GetById(productId);
}

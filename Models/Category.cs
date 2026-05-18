using ClickAndGoApp.DAL;

namespace ClickAndGoApp.Models;

public class Category
{
    private int categoryId;
    private string name;
    private SortedDictionary<string, Product> products = new SortedDictionary<string, Product>();

    public int CategoryId
    {
        get => categoryId;
        set => categoryId = value > 0
            ? value
            : throw new ArgumentException("CategoryId must be positive");
    }

    public string Name
    {
        get => name;
        set => name = !string.IsNullOrWhiteSpace(value)
            ? value
            : throw new ArgumentException("Name cannot be empty");
    }

    public SortedDictionary<string, Product> Products
    {
        get => products;
        set => products = value;
    }

    public Category(int categoryId, string name)
    {
        CategoryId = categoryId;
        Name       = name;
    }

    //==============================
    public static async Task<List<Category>> GetAllAsync(ICategoryDAL dal)
        => await dal.GetAllAsync();

    public static async Task<List<Product>> GetByCategoryAsync(int categoryId, IProductDAL dal) 
        => await dal.GetByCategoryAsync(categoryId);
    
    //==============================
}

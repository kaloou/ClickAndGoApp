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

    public async Task<List<Product>> GetProductsAsync(IProductDAL dal)
    {
        var list = await dal.GetByCategoryAsync(categoryId);
        products.Clear();
        foreach (var p in list)
            products[p.Name] = p;
        return list;
    }

    public void AddProduct(Product product)
    {
        products[product.Name] = product;
    }

    public void RemoveProduct(Product product)
    {
        products.Remove(product.Name);
    }

    //==============================
}

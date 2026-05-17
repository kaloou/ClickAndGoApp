using ClickAndGoApp.DAL.interfaces;

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

    public static Task<List<Category>> GetAll(ICategoryDAL dal) =>
        dal.GetAll();

    public static Task<List<Product>> GetByCategory(int categoryId, IProductDAL dal) =>
        dal.GetByCategory(categoryId);
}

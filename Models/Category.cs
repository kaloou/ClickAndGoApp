namespace ClickAndGoApp.Models;

public class Category
{

    private int categoryId;
    private string name;
    public int CategoryId
    {
        get { return categoryId;}
        set => categoryId = value > 0
            ? value
            : throw new ArgumentException("CategoryId must be positive");
    }
    public string Name
    {
        get { return name; }
        set => name = !string.IsNullOrWhiteSpace(value)
            ? value
            : throw new ArgumentException("Name cannot be empty");
    }

    public Category(int categoryId, string name)
    {
        CategoryId = categoryId;
        Name = name;
    }
}
namespace ClickAndGoApp.Models;

public class RecipeIngredient
{
    private int quantity;
    private Product product;
    private Recipe? recipe;

    // ProductId is derived from the Product object to avoid storing it twice.
    public int ProductId => product.ProductId;

    public int Quantity
    {
        get => quantity;
        set => quantity = value > 0
            ? value
            : throw new ArgumentException("Quantity must be positive");
    }

    public Product Product
    {
        get => product;
        set => product = value ?? throw new ArgumentNullException("Product cannot be null");
    }

    public Recipe? Recipe
    {
        get => recipe;
        set => recipe = value;
    }

    public RecipeIngredient(Product product, int quantity)
    {
        Product  = product;
        Quantity = quantity;
    }

}

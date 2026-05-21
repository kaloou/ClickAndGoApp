namespace ClickAndGoApp.Models;

public class RecipeIngredient
{
    private int quantity;
    private Product product;
    private Recipe? recipe;

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

    public override string ToString()
        => $"[RecipeIngredient] Product={Product?.Name} | Qty={Quantity}";

    public override bool Equals(object obj)
    {
        if (obj is not RecipeIngredient other) return false;
        return ProductId == other.ProductId;
    }

    public override int GetHashCode() => ProductId.GetHashCode();
}

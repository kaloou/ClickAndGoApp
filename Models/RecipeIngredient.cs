namespace ClickAndGoApp.Models;

public class RecipeIngredient
{
    private int recipeId;
    private int productId;
    private int quantity;
    private Product product;

    public int RecipeId
    {
        get => recipeId;
        set => recipeId = value > 0
            ? value
            : throw new ArgumentException("RecipeId must be positive");
    }

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

    public RecipeIngredient(int recipeId, Product product, int quantity)
    {
        RecipeId  = recipeId;
        productId = product.ProductId;
        Product   = product;
        Quantity  = quantity;
    }

    public RecipeIngredient(int recipeId, int productId, int quantity, Product product)
    {
        RecipeId       = recipeId;
        this.productId = productId;
        Quantity       = quantity;
        Product        = product;
    }

    public override string ToString()
        => $"[RecipeIngredient] RecipeId={RecipeId} | Product={Product?.Name} | Qty={Quantity}";

    // Composite key equality — a recipe ingredient is uniquely identified by its recipe + product combination.
    public override bool Equals(object obj)
    {
        if (obj is not RecipeIngredient other) return false;
        return RecipeId == other.RecipeId && ProductId == other.ProductId;
    }

    public override int GetHashCode() => HashCode.Combine(RecipeId, ProductId);
}

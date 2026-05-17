namespace ClickAndGoApp.Models;

public class RecipeIngredient
{
    private int recipeId;
    private Product product;
    private int quantity;

    public int RecipeId
    {
        get => recipeId;
        set => recipeId = value > 0
            ? value
            : throw new ArgumentException("RecipeId must be positive");
    }

    public Product Product
    {
        get => product;
        set => product = value ?? throw new ArgumentNullException("Product cannot be null");
    }

    public int Quantity
    {
        get => quantity;
        set => quantity = value > 0
            ? value
            : throw new ArgumentException("Quantity must be positive");
    }

    public RecipeIngredient(int recipeId, Product product, int quantity)
    {
        RecipeId = recipeId;
        Product  = product;
        Quantity = quantity;
    }
}

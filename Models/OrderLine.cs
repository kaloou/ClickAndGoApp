using ClickAndGoApp.DAL;

namespace ClickAndGoApp.Models;

// Represents a single product line in an order, with its quantity.
public class OrderLine
{
    private int quantity;
    private Product product;

    // Delegates to the product object — avoids storing a redundant productId field.
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

    public OrderLine(int quantity, Product product)
    {
        Quantity = quantity;
        Product  = product;
    }

    public Product GetProduct() => product;

    public async Task RemoveAsync(int orderId, IOrderLineDAL dal) =>
        await dal.RemoveAsync(orderId, product.ProductId);

    public async Task SetQuantityAsync(int orderId, int quantity, IOrderLineDAL dal) =>
        await dal.SetQuantityAsync(orderId, product.ProductId, quantity);

}

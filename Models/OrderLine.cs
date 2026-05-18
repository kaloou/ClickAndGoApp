using ClickAndGoApp.DAL;

namespace ClickAndGoApp.Models;

public class OrderLine
{
    private int orderId;
    private int productId;
    private int quantity;
    private Product product;

    public int OrderId
    {
        get => orderId;
        set => orderId = value > 0
            ? value
            : throw new ArgumentException("OrderId must be positive");
    }

    public int ProductId
    {
        get => productId;
        set => productId = value > 0
            ? value
            : throw new ArgumentException("ProductId must be positive");
    }

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

    // Constructeur kalou : productId déduit du product
    public OrderLine(int orderId, int quantity, Product product)
    {
        OrderId   = orderId;
        ProductId = product.ProductId;
        Quantity  = quantity;
        Product   = product;
    }

    // Constructeur bywaa : productId explicite
    public OrderLine(int orderId, int productId, int quantity, Product product)
    {
        OrderId   = orderId;
        ProductId = productId;
        Quantity  = quantity;
        Product   = product;
    }

    public Product GetProduct() => product;

    public static async Task<List<OrderLine>> GetOrderLinesAsync(int orderId, IOrderLineDAL dal) =>
        await dal.GetOrderLinesAsync(orderId);

    public async Task RemoveAsync(IOrderLineDAL dal) =>
        await dal.RemoveAsync(orderId, product.ProductId);

    public async Task SetQuantityAsync(int quantity, IOrderLineDAL dal) =>
        await dal.SetQuantityAsync(orderId, product.ProductId, quantity);

    public override string ToString() =>
        $"[OrderLine] OrderId={OrderId} | Product={Product?.Name} | Qty={Quantity}";

    public override bool Equals(object obj)
    {
        if (obj is not OrderLine other) return false;
        return OrderId == other.OrderId && ProductId == other.ProductId;
    }

    public override int GetHashCode() => HashCode.Combine(OrderId, ProductId);
}

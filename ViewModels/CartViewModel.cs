using ClickAndGoApp.Models;

namespace ClickAndGoApp.ViewModels;

public record CartViewModel(Order Order, List<OrderLine> OrderLines)
{
    public float ProductsTotal => OrderLines.Sum(ol => ol.Product.Price * ol.Quantity);
    public float Total => Order?.ComputeTotal(ProductsTotal) ?? 0f;
}

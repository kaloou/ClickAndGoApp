using ClickAndGoApp.Models;

namespace ClickAndGoApp.ViewModels;

public class CartViewModel
{
    public Order Order { get; set; }
    public List<OrderLine> OrderLines { get; set; }

    public float ProductsTotal
    {
        get
        {
            float total = 0;
            foreach (OrderLine ol in OrderLines)
                total += ol.Product.Price * ol.Quantity;
            return total;
        }
    }
    public float Total => Order.ComputeTotal(ProductsTotal);
}

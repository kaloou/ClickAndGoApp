using ClickAndGoApp.Models;

namespace ClickAndGoApp.ViewModels;

public class OrderDetailsViewModel
{
    public Order Order { get; set; }
    public List<OrderLine> OrderLines { get; set; }
    public List<Product> Products { get; set; }
}

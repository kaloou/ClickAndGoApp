using ClickAndGoApp.Models;

namespace ClickAndGoApp.ViewModels;

public record OrderDetailsViewModel(Order Order, List<OrderLine> OrderLines, List<Product> Products);

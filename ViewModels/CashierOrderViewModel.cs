using ClickAndGoApp.Models;

namespace ClickAndGoApp.ViewModels;

public record CashierOrderViewModel(Order Order, float TotalAmount);

using ClickAndGoApp.Models;

namespace ClickAndGoApp.ViewModels;

public record CustomerAccountViewModel(Customer Customer, List<Order> Orders);

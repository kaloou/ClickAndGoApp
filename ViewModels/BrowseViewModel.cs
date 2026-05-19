using ClickAndGoApp.Models;

namespace ClickAndGoApp.ViewModels;

public record BrowseViewModel(List<Product> Products, List<Category> Categories, int? SelectedCategoryId);

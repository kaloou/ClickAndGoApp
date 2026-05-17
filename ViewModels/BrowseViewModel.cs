using ClickAndGoApp.Models;

namespace ClickAndGoApp.ViewModels;
public class BrowseViewModel
{
    public List<Product> Products { get; set; }
    public List<Category> Categories { get; set; }
    public int? SelectedCategoryId { get; set; }
}

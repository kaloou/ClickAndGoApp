namespace ClickAndGoApp.Models;

public class BrowseViewModel
{
    public List<Product> Products { get; set; }
    public List<Category> Categories { get; set; }
    public string SearchQuery { get; set; }
    public int? SelectedCategoryId { get; set; }
}

namespace ClickAndGoApp.Models.ViewModels
{
    public class OrderDetailsViewModel
    {
        public Order Order { get; set; }
        public List<OrderLine> OrderLines { get; set; }
    }
}

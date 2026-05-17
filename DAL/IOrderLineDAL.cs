using ClickAndGoApp.Models;

namespace ClickAndGoApp.DAL
{
    public interface IOrderLineDAL
    {
        Task<float> GetProductsTotalAsync(int orderId);
        Task<List<OrderLine>> GetOrderLinesAsync(int orderId);
    }
}

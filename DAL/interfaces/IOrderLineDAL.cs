using ClickAndGoApp.Models;

namespace ClickAndGoApp.DAL;

public interface IOrderLineDAL
{
    Task<float> GetProductsTotalAsync(int orderId);
    Task<List<OrderLine>> GetOrderLinesAsync(int orderId);
    Task AddProductAsync(int orderId, int productId, int quantity);
    Task RemoveAsync(int orderId, int productId);
    Task SetQuantityAsync(int orderId, int productId, int quantity);
}

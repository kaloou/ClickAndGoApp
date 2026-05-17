using ClickAndGoApp.Models;

namespace ClickAndGoApp.DAL.interfaces;

public interface IOrderLineDAL
{
    Task<List<OrderLine>> GetOrderLines(int orderId);
    Task AddProduct(int orderId, int productId, int quantity);
    Task Remove(int orderId, int productId);
    Task SetQuantity(int orderId, int productId, int quantity);
}

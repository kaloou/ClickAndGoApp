using ClickAndGoApp.Models;
using ClickAndGoApp.Models.Enums;

namespace ClickAndGoApp.DAL;

public interface IOrderDAL
{
    Task<Order> GetByIdAsync(int orderId);
    Task<List<Order>> GetOrdersByStoreAsync(int storeId);
    Task<List<Order>> GetTodaysOrdersAsync(int storeId);
    Task SetNumberOfBoxesAsync(int orderId, int numberOfBoxes);
    Task SetReturnedBoxesAsync(int orderId, int returnedBoxes);
    Task SetStatusAsync(int orderId, OrderStatus status);
    Task<int> CreateOrderAsync(int customerId);
    Task<int?> GetActiveCartAsync(int customerId);
    Task<List<Order>> GetOrdersByCustomerAsync(int customerId);
    Task SetTimeSlotAsync(int orderId, int timeSlotId);
}

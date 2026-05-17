using ClickAndGoApp.Models;
using ClickAndGoApp.Models.Enums;

namespace ClickAndGoApp.DAL.interfaces;

public interface IOrderDAL
{
    public Task<int> CreateOrder(int customerId);
    public Task<Order> GetById(int orderId);
    public Task<List<Order>> GetOrdersByStore(int storeId);
    public Task<List<Order>> GetOrdersByCustomer(int customerId);
    public Task<List<Order>> GetOrdersToPrepare(int storeId);
    public Task<List<Order>> GetTodayOrders(int storeId);
    public Task SetTimeSlot(int orderId, int timeSlotId);
    public Task SetStatus(int orderId, OrderStatus status);
    public Task SetNumberOfBoxes(int orderId, int numberOfBoxes);
    public Task SetReturnedBoxes(int orderId, int returnedBoxes);
}

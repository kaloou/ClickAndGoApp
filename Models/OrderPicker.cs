using ClickAndGoApp.DAL;

namespace ClickAndGoApp.Models;

public class OrderPicker : Employee
{
    public OrderPicker(int userId, string firstName, string lastName,
        string email, string password, Store store)
        : base(userId, firstName, lastName, email, password, store)
    {
        Role = "OrderPicker";
    }

    public static async Task<OrderPicker> GetByIdAsync(int pickerId, IOrderPickerDAL dal)
        => await dal.GetByIdAsync(pickerId);

    // The store is already loaded when the OrderPicker is fetched from the DB (via JOIN in the DAL),
    // so we wrap it in a completed Task to match the async interface without an extra DB call.
    public Task<Store> GetStoreAsync() => Task.FromResult(Store);
}

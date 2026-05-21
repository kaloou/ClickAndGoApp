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

    //==============================
    public static async Task<OrderPicker> GetByIdAsync(int pickerId, IOrderPickerDAL dal)
        => await dal.GetByIdAsync(pickerId);

    public Task<Store> GetStoreAsync() => Task.FromResult(Store);

    //==============================

    public override string ToString() =>
        $"[OrderPicker] Id={UserId} | {FirstName} {LastName} | Store={Store.StoreId}";
}

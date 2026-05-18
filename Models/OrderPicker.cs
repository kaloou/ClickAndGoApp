using ClickAndGoApp.DAL;

namespace ClickAndGoApp.Models;

public class OrderPicker : Employee
{
    public OrderPicker(int userId, string firstName, string lastName,
        string email, string password, int storeId)
        : base(userId, firstName, lastName, email, password, storeId)
    {
    }
    
    //==============================
    public static async Task<OrderPicker> GetByIdAsync(int pickerId, IOrderPickerDAL dal) =>
        await dal.GetByIdAsync(pickerId);
    
    public override async Task<Store> GetStoreAsync(IStoreDAL dal) =>
        await base.GetStoreAsync(dal);
    
    //==============================
    
    public override string ToString() =>
        $"[OrderPicker] Id={UserId} | {FirstName} {LastName} | Store={StoreId}";
}

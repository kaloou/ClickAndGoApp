using ClickAndGoApp.DAL;

namespace ClickAndGoApp.Models;

public class Cashier : Employee
{
    public Cashier(int userId, string firstName, string lastName,
        string email, string password, int storeId)
        : base(userId, firstName, lastName, email, password, storeId)
    {
    }
    
    //==============================
    public static async Task<Cashier> GetByIdAsync(int cashierId, ICashierDAL dal) 
        => await dal.GetByIdAsync(cashierId);

    public override async Task<Store> GetStoreAsync(IStoreDAL dal) =>
        await base.GetStoreAsync(dal);
    
    //==============================

    public override string ToString()
        => $"[Cashier] Id={UserId} | {FirstName} {LastName} | Store={StoreId}";
}

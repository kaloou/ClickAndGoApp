using ClickAndGoApp.DAL;

namespace ClickAndGoApp.Models;

public abstract class Employee : User
{
    private int storeId;
    private Store store;

    public int StoreId
    {
        get => storeId;
        set => storeId = value > 0
            ? value
            : throw new ArgumentException("StoreId must be positive");
    }

    public Store Store
    {
        get => store;
        set => store = value ?? throw new ArgumentNullException("Store cannot be null");
    }

    public Employee(int userId, string firstName, string lastName,
        string email, string password, int storeId)
        : base(userId, firstName, lastName, email, password)
    {
        StoreId = storeId;
    }

    //==============================
    public virtual async Task<Store> GetStoreAsync(IStoreDAL dal) =>
        await Store.GetStoreAsync(StoreId, dal);
    
    //==============================
    
}

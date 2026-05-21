using ClickAndGoApp.DAL;

namespace ClickAndGoApp.Models;

public class Cashier : Employee
{
    public Cashier(int userId, string firstName, string lastName,
        string email, string password, Store store)
        : base(userId, firstName, lastName, email, password, store)
    {
        Role = "Cashier";
    }

    public static async Task<Cashier> GetByIdAsync(int cashierId, ICashierDAL dal)
        => await dal.GetByIdAsync(cashierId);

    // Same reasoning as OrderPicker.GetStoreAsync — store is already in memory, no DB roundtrip needed.
    public Task<Store> GetStoreAsync() => Task.FromResult(Store);

    public override string ToString()
        => $"[Cashier] Id={UserId} | {FirstName} {LastName} | Store={Store.StoreId}";
}

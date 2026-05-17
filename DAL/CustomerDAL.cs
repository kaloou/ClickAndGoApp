using ClickAndGoApp.DAL.interfaces;
using ClickAndGoApp.Models;

namespace ClickAndGoApp.DAL;

public class CustomerDAL : ICustomerDAL
{
    private readonly DBConnection db;

    public CustomerDAL(DBConnection db)
    {
        this.db = db;
    }

    public Task<bool> GetByEmail(string email) => Task.FromResult(false);

    public Task<Customer> CreateAccount(string name, string email, string password) => Task.FromResult<Customer>(null);
}

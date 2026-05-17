using ClickAndGoApp.Models;

namespace ClickAndGoApp.DAL.interfaces;

public interface ICustomerDAL
{
    Task<bool> GetByEmail(string email);
    Task<Customer> CreateAccount(string name, string email, string password);
}

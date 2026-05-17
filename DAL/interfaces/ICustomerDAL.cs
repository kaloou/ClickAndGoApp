using ClickAndGoApp.Models;

namespace ClickAndGoApp.DAL.interfaces;

public interface ICustomerDAL
{
    Task<bool> GetByEmail(string email);
    Task<Customer> CreateAccount(string firstName, string lastName, string email, string password, string? phoneNumber, string? address);
}

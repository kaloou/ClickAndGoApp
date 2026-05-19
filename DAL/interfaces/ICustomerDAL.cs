using ClickAndGoApp.Models;

namespace ClickAndGoApp.DAL;

public interface ICustomerDAL
{
    Task<bool> GetByEmailAsync(string email);
    Task<Customer> CreateAccountAsync(string firstName, string lastName, string email, string password, string? phoneNumber, string? address);
    Task<Customer> GetByIdAsync(int userId);
    Task UpdateAsync(int userId, string firstName, string lastName, int phoneNumber, string? address, string password);
}

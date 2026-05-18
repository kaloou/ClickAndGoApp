using ClickAndGoApp.Models;

namespace ClickAndGoApp.DAL;

public interface IUserDAL
{
    Task<User> GetByCredentialsAsync(string email, string password);
}

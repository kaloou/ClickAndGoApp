using ClickAndGoApp.Models;

namespace ClickAndGoApp.DAL.interfaces;

public interface IUserDAL
{
    Task<User> GetByCredentials(string email, string password);
}

using ClickAndGoApp.Models;

namespace ClickAndGoApp.DAL;

public interface ICategoryDAL
{
    Task<List<Category>> GetAllAsync();
}

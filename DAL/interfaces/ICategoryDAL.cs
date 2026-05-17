using ClickAndGoApp.Models;

namespace ClickAndGoApp.DAL.interfaces;

public interface ICategoryDAL
{
    public Task<List<Category>> GetAll();
}

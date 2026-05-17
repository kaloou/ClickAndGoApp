using ClickAndGoApp.Models;

namespace ClickAndGoApp.DAL.interfaces;

public interface IProductDAL
{
    Task<List<Product>> GetAll();
    Task<List<Product>> GetByCategory(int categoryId);
    Task<Product> GetById(int productId);
}

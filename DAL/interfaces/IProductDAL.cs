using ClickAndGoApp.Models;

namespace ClickAndGoApp.DAL;

public interface IProductDAL
{
    Task<List<Product>> GetAllAsync();
    Task<List<Product>> GetByCategoryAsync(int categoryId);
    Task<Product> GetByIdAsync(int productId);
}

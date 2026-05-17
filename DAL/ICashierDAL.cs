using ClickAndGoApp.Models;

namespace ClickAndGoApp.DAL
{
    public interface ICashierDAL
    {
        Task<Cashier> GetByIdAsync(int cashierId);
    }
}

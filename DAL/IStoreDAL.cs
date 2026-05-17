using ClickAndGoApp.Models;

namespace ClickAndGoApp.DAL
{
    public interface IStoreDAL
    {
        Task<Store> GetStoreAsync(int storeId);
    }
}

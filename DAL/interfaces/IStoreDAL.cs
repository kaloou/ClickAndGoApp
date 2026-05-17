using ClickAndGoApp.Models;

namespace ClickAndGoApp.DAL.interfaces;

public interface IStoreDAL
{
    Task<Store> GetStore(int storeId);
    Task<List<Store>> GetAllStores();
    Task<List<TimeSlot>> GetAvailableTimeSlots(int storeId);
}

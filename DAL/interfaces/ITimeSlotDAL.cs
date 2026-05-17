using ClickAndGoApp.Models;

namespace ClickAndGoApp.DAL.interfaces;

public interface ITimeSlotDAL
{
    Task<List<TimeSlot>> GetByStore(int storeId);
}

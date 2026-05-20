using ClickAndGoApp.Models;

namespace ClickAndGoApp.DAL;

public interface ITimeSlotDAL
{
    Task<List<TimeSlot>> GetAvailableTimeSlotsAsync(int storeId);
}

using ClickAndGoApp.DAL.interfaces;
using ClickAndGoApp.Models;

namespace ClickAndGoApp.DAL;

public class TimeSlotDAL : ITimeSlotDAL
{
    private readonly DBConnection db;

    public TimeSlotDAL(DBConnection db)
    {
        this.db = db;
    }

    public Task<List<TimeSlot>> GetByStore(int storeId) => Task.FromResult<List<TimeSlot>>(null);
}

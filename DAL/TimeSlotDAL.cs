namespace ClickAndGoApp.DAL;

public class TimeSlotDAL : ITimeSlotDAL
{
    private readonly DBConnection db;

    public TimeSlotDAL(DBConnection db)
    {
        this.db = db;
    }
}

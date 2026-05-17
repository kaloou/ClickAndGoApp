namespace ClickAndGoApp.Models;

public class TimeSlot
{
    private int timeSlotId;
    private DateTime startTime;
    private DateTime endTime;

    public int TimeSlotId
    {
        get => timeSlotId;
        set => timeSlotId = value > 0
            ? value
            : throw new ArgumentException("TimeSlotId must be positive");
    }

    public DateTime StartTime
    {
        get => startTime;
        set => startTime = value != default
            ? value
            : throw new ArgumentException("StartTime is not valid");
    }

    public DateTime EndTime
    {
        get => endTime;
        set => endTime = value != default
            ? value
            : throw new ArgumentException("EndTime is not valid");
    }

    public TimeSlot(int timeSlotId, DateTime startTime, DateTime endTime)
    {
        TimeSlotId = timeSlotId;
        StartTime  = startTime;
        EndTime    = endTime;
    }
}

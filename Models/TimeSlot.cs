namespace ClickAndGoApp.Models;

// Represents a pickup window at a store (e.g. 10:00–10:30).
public class TimeSlot
{
    private int timeSlotId;
    private DateTime startTime;
    private DateTime endTime;
    private Queue<Order> orders;

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

    // EndTime must be strictly after StartTime — a zero-length or backwards slot is invalid.
    public DateTime EndTime
    {
        get => endTime;
        set
        {
            if (value != default && value > startTime)
                endTime = value;
            else
                throw new ArgumentException("EndTime must be after StartTime");
        }
    }

    public Queue<Order> Orders
    {
        get => orders;
        set => orders = value ?? throw new ArgumentNullException("Orders cannot be null");
    }

    public TimeSlot(int timeSlotId, DateTime startTime, DateTime endTime)
    {
        TimeSlotId = timeSlotId;
        StartTime  = startTime;
        EndTime    = endTime;
        orders     = new Queue<Order>();
    }

}

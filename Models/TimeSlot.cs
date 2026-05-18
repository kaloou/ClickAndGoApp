namespace ClickAndGoApp.Models;

public class TimeSlot
{
    private int timeSlotId;
    private DateTime startTime;
    private DateTime endTime;
    private Queue<Order> orders = new Queue<Order>();

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
    }

    public void AddOrder(Order order)
    {
        if (orders.Count >= 10)
            throw new InvalidOperationException("A time slot cannot have more than 10 orders");
        orders.Enqueue(order);
    }
}

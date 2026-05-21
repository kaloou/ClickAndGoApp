using ClickAndGoApp.DAL;

namespace ClickAndGoApp.Models;

public class Store
{
    private int storeId;
    private string name;
    private string address;
    private List<Order> orders;
    private Dictionary<int, Employee> employees;
    private SortedList<DateTime, TimeSlot> timeSlots;

    public int StoreId
    {
        get => storeId;
        set => storeId = value > 0
            ? value
            : throw new ArgumentException("StoreId must be positive");
    }

    public string Name
    {
        get => name;
        set => name = !string.IsNullOrWhiteSpace(value)
            ? value
            : throw new ArgumentException("Name cannot be empty");
    }

    public string Address
    {
        get => address;
        set => address = !string.IsNullOrWhiteSpace(value)
            ? value
            : throw new ArgumentException("Address cannot be empty");
    }

    public List<Order> Orders
    {
        get => orders;
        set => orders = value;
    }

    public Dictionary<int, Employee> Employees
    {
        get => employees;
        set => employees = value ?? throw new ArgumentNullException("Employees cannot be null");
    }

    public SortedList<DateTime, TimeSlot> TimeSlots
    {
        get => timeSlots;
        set => timeSlots = value ?? throw new ArgumentNullException("TimeSlots cannot be null");
    }

    public Store(int storeId, string name, string address)
    {
        StoreId   = storeId;
        Name      = name;
        Address   = address;
        // A List is used for orders since we need index access and the order doesn't matter.
        orders    = new List<Order>();
        // Dictionary lookup by employee ID.
        employees = new Dictionary<int, Employee>();
        // SortedList keeps time slots sorted by start time automatically — no manual sort needed.
        timeSlots = new SortedList<DateTime, TimeSlot>();
    }

    public async Task<List<Order>> GetOrdersByStoreAsync(IOrderDAL dal)
    {
        orders.Clear();
        foreach (var order in await dal.GetOrdersByStoreAsync(storeId))
            AddOrder(order);
        return orders;
    }

    public async Task<List<Order>> GetTodaysOrdersAsync(IOrderDAL dal)
    {
        orders.Clear();
        foreach (var order in await dal.GetTodaysOrdersAsync(storeId))
            AddOrder(order);
        return orders;
    }

    public async Task<List<TimeSlot>> GetAvailableTimeSlotsAsync(ITimeSlotDAL dal)
    {
        var list = await dal.GetAvailableTimeSlotsAsync(storeId);
        timeSlots.Clear();
        foreach (var ts in list)
            AddTimeSlot(ts);
        return list;
    }

    public void AddOrder(Order order)
    {
        if (!orders.Contains(order))
            orders.Add(order);
    }

    public void RemoveOrder(Order order)
    {
        orders.Remove(order);
    }

    public void AddTimeSlot(TimeSlot timeSlot)
    {
        timeSlots[timeSlot.StartTime] = timeSlot;
    }

    public static async Task<List<Store>> GetAllStoresAsync(IStoreDAL dal)
        => await dal.GetAllStoresAsync();

    //==============================

    public static async Task<Store> GetStoreAsync(int storeId, IStoreDAL dal)
        => await dal.GetStoreAsync(storeId);
}

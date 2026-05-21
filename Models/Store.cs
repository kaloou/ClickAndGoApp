using ClickAndGoApp.DAL;

namespace ClickAndGoApp.Models;

public class Store
{
    private int storeId;
    private string name;
    private string address;
    // A List is used for orders since we need index access and the order doesn't matter.
    private List<Order> orders = new List<Order>();
    private Dictionary<int, Employee> employees = new Dictionary<int, Employee>();
    // A SortedList keeps time slots sorted by start time automatically — no manual sort needed.
    private SortedList<DateTime, TimeSlot> timeSlots = new SortedList<DateTime, TimeSlot>();

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
        StoreId = storeId;
        Name    = name;
        Address = address;
    }

    public async Task<List<Order>> GetOrdersByStoreAsync(IOrderDAL dal)
    {
        orders = await dal.GetOrdersByStoreAsync(storeId);
        return orders;
    }

    public async Task<List<Order>> GetTodaysOrdersAsync(IOrderDAL dal)
    {
        orders = await dal.GetTodaysOrdersAsync(storeId);
        return orders;
    }

    public async Task<List<TimeSlot>> GetAvailableTimeSlotsAsync(ITimeSlotDAL dal)
    {
        var list = await dal.GetAvailableTimeSlotsAsync(storeId);
        timeSlots.Clear();
        foreach (var ts in list)
            timeSlots[ts.StartTime] = ts;
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

    public void AddEmployee(Employee employee)
    {
        employees[employee.UserId] = employee;
    }

    public void RemoveEmployee(int userId)
    {
        employees.Remove(userId);
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

    public override string ToString()
        => $"[Store] Id={StoreId} | {Name} | {Address}";

    public override bool Equals(object obj)
    {
        if (obj is not Store other) return false;
        return StoreId == other.StoreId;
    }

    public override int GetHashCode() => StoreId.GetHashCode();
}

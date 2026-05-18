using ClickAndGoApp.DAL;

namespace ClickAndGoApp.Models;

public class Store
{
    private int storeId;
    private string name;
    private string address;
    private List<Order> orders = new List<Order>();
    private Dictionary<int, Employee> employees = new Dictionary<int, Employee>();
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
    
    //==============================
    public async Task<List<Order>> GetOrdersByStoreAsync(IOrderDAL dal)
        => await dal.GetOrdersByStoreAsync(storeId);
    
    public async Task<List<Order>> GetTodaysOrdersAsync(IOrderDAL dal) 
        => await dal.GetTodaysOrdersAsync(storeId);
    
    public async Task<List<TimeSlot>> GetAvailableTimeSlotsAsync(IStoreDAL dal) 
        => await dal.GetAvailableTimeSlotsAsync(storeId);
    
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

using ClickAndGoApp.DAL.interfaces;

namespace ClickAndGoApp.Models;

public class Store
{
    private int storeId;
    private string name;
    private string address;
    private List<Order> orders;
    private List<TimeSlot> timeSlots;

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

    public List<TimeSlot> TimeSlots
    {
        get => timeSlots;
        set => timeSlots = value;
    }

    public Store(int storeId, string name, string address)
    {
        StoreId = storeId;
        Name    = name;
        Address = address;
    }

    //methodes
    public static async Task<List<Store>> GetAllStoresAsync(IStoreDAL dal) => //
        await dal.GetAllStores();

    public async Task<List<Order>> GetOrdersByStoreAsync(DateOnly date, IOrderDAL dal) => //
        await dal.GetOrdersByStore(storeId);

    public async Task<List<Order>> GetTodayOrdersAsync(IOrderDAL dal) => //
        await dal.GetTodayOrders(storeId);

    public async Task<List<TimeSlot>> GetAvailableTimeSlotsAsync(IStoreDAL dal) => //
        await dal.GetAvailableTimeSlots(storeId);
}

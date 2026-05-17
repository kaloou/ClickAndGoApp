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
    public static Task<List<Store>> GetAllStores(IStoreDAL dal) => //
        dal.GetAllStores();

    public Task<List<TimeSlot>> GetAvailableTimeSlots(IStoreDAL dal) => //
        dal.GetAvailableTimeSlots(storeId);

    public Task<List<Order>> GetOrdersByStore(DateOnly date, IOrderDAL dal) => //
        dal.GetOrdersByStore(storeId);

    public Task<List<Order>> GetTodayOrders(IOrderDAL dal) => //
        dal.GetTodayOrders(storeId);
}

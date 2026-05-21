using ClickAndGoApp.DAL;
using ClickAndGoApp.Models.Enums;

namespace ClickAndGoApp.Models;

// IDisposable is implemented because an Order holds a list of OrderLines in memory.
// Calling Dispose clears that list and allows the GC to reclaim the memory sooner,
// which matters when many orders are loaded at once (e.g. the picker's order list).
public class Order : IDisposable
{
    private int orderId;
    private DateTime orderDate;
    private OrderStatus status;
    private int numberOfBoxes;
    private int returnedBoxes;
    private DateTime pickupDate;
    private PaymentStatus paymentStatus;
    private bool isSelected;
    private Customer customer;
    private TimeSlot? timeSlot;
    private Store? store;
    // Initialized to an empty list so callers can always safely iterate without null checks.
    private List<OrderLine> orderLines = new List<OrderLine>();

    private bool disposed = false;
    
    public const float SERVICE_FEE  = 5.95f;
    public const float BOX_DEPOSIT  = 5.95f;

    public int OrderId
    {
        get => orderId;
        set => orderId = value > 0
            ? value
            : throw new ArgumentException("OrderId must be positive");
    }

    public DateTime OrderDate
    {
        get => orderDate;
        set => orderDate = value != default
            ? value
            : throw new ArgumentException("OrderDate is not valid");
    }

    public OrderStatus Status
    {
        get => status;
        set => status = value;
    }

    public int NumberOfBoxes
    {
        get => numberOfBoxes;
        set => numberOfBoxes = value >= 0
            ? value
            : throw new ArgumentException("NumberOfBoxes cannot be negative");
    }

    public int ReturnedBoxes
    {
        get => returnedBoxes;
        set => returnedBoxes = value >= 0
            ? value
            : throw new ArgumentException("ReturnedBoxes cannot be negative");
    }

    public DateTime PickupDate
    {
        get => pickupDate;
        set => pickupDate = value;
    }

    public PaymentStatus PaymentStatus
    {
        get => paymentStatus;
        set => paymentStatus = value;
    }

    // Expose the customer's ID directly so callers don't have to navigate through Customer.
    public int CustomerId => customer.UserId;

    // Returns 0 when no time slot is assigned yet
    public int TimeSlotId => timeSlot?.TimeSlotId ?? 0;

    public Customer Customer
    {
        get => customer;
        set => customer = value ?? throw new ArgumentNullException("Customer cannot be null");
    }

    public TimeSlot? TimeSlot
    {
        get => timeSlot;
        set => timeSlot = value;
    }

    public Store? Store
    {
        get => store;
        set => store = value;
    }

    public List<OrderLine> OrderLines
    {
        get => orderLines;
        set => orderLines = value;
    }

    public bool IsSelected => isSelected;

    public Order(int orderId, DateTime orderDate, OrderStatus status,
                 int numberOfBoxes, int returnedBoxes, DateTime pickupDate,
                 PaymentStatus paymentStatus, Customer customer, TimeSlot? timeSlot, Store? store)
    {
        OrderId       = orderId;
        OrderDate     = orderDate;
        Status        = status;
        NumberOfBoxes = numberOfBoxes;
        ReturnedBoxes = returnedBoxes;
        PickupDate    = pickupDate;
        PaymentStatus = paymentStatus;
        Customer      = customer;
        TimeSlot      = timeSlot;
        Store         = store;
    }

    // OrderLines are loaded on demand rather than in the constructor to avoid
    // fetching data we don't always need (e.g. when just listing orders).
    public async Task<List<OrderLine>> GetOrderLinesAsync(IOrderLineDAL dal)
        => await dal.GetOrderLinesAsync(orderId);

    public async Task SetNumberOfBoxesAsync(int numberOfBoxes, IOrderDAL dal)
        => await dal.SetNumberOfBoxesAsync(orderId, numberOfBoxes);

    public static async Task<Order> GetByIdAsync(int orderId, IOrderDAL dal)
        => await dal.GetByIdAsync(orderId);

    // Marks the order as selected in the cashier's view — used to highlight it in the UI.
    public Order GetSelected(bool selected)
    {
        isSelected = selected;
        return this;
    }

    public async Task SetReturnedBoxesAsync(int returnedBoxes, IOrderDAL dal) 
        => await dal.SetReturnedBoxesAsync(orderId, returnedBoxes);

    // Total = products subtotal + service fee + (boxes given × deposit) - (boxes returned × deposit).
    // The deposit is refunded for each box the customer brings back.
    public float ComputeTotal(float productsTotal) 
        => productsTotal + SERVICE_FEE + (NumberOfBoxes * BOX_DEPOSIT) - (ReturnedBoxes * BOX_DEPOSIT);

    // Async version that fetches the products subtotal from the DB before computing.
    public async Task<float> ComputeTotalAsync(IOrderLineDAL dal)
    {
        float productsTotal = await dal.GetProductsTotalAsync(orderId);
        return ComputeTotal(productsTotal);
    }

    public async Task SetStatusAsync(OrderStatus status, IOrderDAL dal) 
        => await dal.SetStatusAsync(orderId, status);

    // SetStore is called after fetching orders by store — the store isn't in the Order table itself,
    // it's derived from the TimeSlot join, so we backfill it here.
    public void SetStore(int storeId)
    {
        if (store == null)
            store = new Store(storeId, "-", "-"); // placeholder when only the ID matters
        else
            store.StoreId = storeId;
    }

    public async Task SetTimeSlotAsync(int timeSlotId, IOrderDAL dal)
        => await dal.SetTimeSlotAsync(orderId, timeSlotId);

    public async Task AddProductAsync(int productId, IOrderLineDAL dal, int quantity = 1)
        => await dal.AddProductAsync(orderId, productId, quantity);

    public static async Task<List<Order>> GetOrdersByCustomerAsync(int customerId, IOrderDAL dal)
        => await dal.GetOrdersByCustomerAsync(customerId);

    // Dispose(true) is called from public Dispose() — managed resources are cleaned up.
    // Dispose(false) is called from the finalizer — only unmanaged resources would be freed here,
    // but we have none; the finalizer exists as a safety net.
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this); // prevents the finalizer from running since we already cleaned up
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
                orderLines.Clear();
            disposed = true;
        }
    }

    ~Order() => Dispose(false);

    public override string ToString()
        => $"[Order] Id={OrderId} | Status={Status} | PickupDate={PickupDate:dd/MM/yyyy HH:mm} | CustomerId={CustomerId}";

    // Equality is based on the primary key — two Order objects with the same ID represent the same order.
    // This is what makes orders.Remove(order) work correctly in MarkAsCollected.
    public override bool Equals(object obj)
    {
        if (obj is not Order other) return false;
        return OrderId == other.OrderId;
    }

    public override int GetHashCode() => OrderId.GetHashCode();
}

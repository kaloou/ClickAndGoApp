using ClickAndGoApp.DAL;
using ClickAndGoApp.Models.Enums;

namespace ClickAndGoApp.Models;

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
    private List<OrderLine> orderLines = new List<OrderLine>();

    private bool disposed = false;
    
    public const float SERVICE_FEE = 5.95f;
    public const float BOX_DEPOSIT = 5.95f;

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

    public int CustomerId => customer.UserId;

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

    //==============================
    public async Task<List<OrderLine>> GetOrderLinesAsync(IOrderLineDAL dal) 
        => await dal.GetOrderLinesAsync(orderId);
    
    public async Task SetNumberOfBoxesAsync(int numberOfBoxes, IOrderDAL dal) 
        => await dal.SetNumberOfBoxesAsync(orderId, numberOfBoxes);
    
    public static async Task<Order> GetByIdAsync(int orderId, IOrderDAL dal)
        => await dal.GetByIdAsync(orderId);
    
    public Order GetSelected(bool selected)
    {
        isSelected = selected;
        return this;
    }
    
    public async Task SetReturnedBoxesAsync(int returnedBoxes, IOrderDAL dal) =>
        await dal.SetReturnedBoxesAsync(orderId, returnedBoxes);
    
    public float ComputeTotal(float productsTotal) =>
        productsTotal + SERVICE_FEE + (NumberOfBoxes * BOX_DEPOSIT) - (ReturnedBoxes * BOX_DEPOSIT);

    public async Task<float> ComputeTotalAsync(IOrderLineDAL dal)
    {
        float productsTotal = await dal.GetProductsTotalAsync(orderId);
        return ComputeTotal(productsTotal);
    }
    
    public async Task SetStatusAsync(OrderStatus status, IOrderDAL dal) =>
        await dal.SetStatusAsync(orderId, status);
    
    public void SetStore(int storeId)
    {
        if (store == null) // pour les OrderCart
            store = new Store(storeId, "-", "-");
        else
            store.StoreId = storeId;
    }
    
    public async Task SetTimeSlotAsync(int timeSlotId, IOrderDAL dal) =>
        await dal.SetTimeSlotAsync(orderId, timeSlotId);
    
    public async Task AddProductAsync(int productId, IOrderLineDAL dal, int quantity = 1) =>
        await dal.AddProductAsync(orderId, productId, quantity);
    
    public static async Task<List<Order>> GetOrdersByCustomerAsync(int customerId, IOrderDAL dal)
        => await dal.GetOrdersByCustomerAsync(customerId);
    // METHODE REMOVE A IMPLEMENTER
    
    
    //==============================
    
    //==============================
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
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

    //==============================

    public override string ToString()
        => $"[Order] Id={OrderId} | Status={Status} | PickupDate={PickupDate:dd/MM/yyyy HH:mm} | CustomerId={CustomerId}";

    public override bool Equals(object obj)
    {
        if (obj is not Order other) return false;
        return OrderId == other.OrderId;
    }

    public override int GetHashCode() => OrderId.GetHashCode();
}

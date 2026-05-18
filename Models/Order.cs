using ClickAndGoApp.DAL.interfaces;
using ClickAndGoApp.Models.Enums;

namespace ClickAndGoApp.Models;

public class Order
{
    private int orderId;
    private DateTime orderDate;
    private OrderStatus status;
    private int numberOfBoxes;
    private int returnedBoxes;
    private DateTime pickupDate;    // DateTime.MinValue pour les orders cart
    private PaymentStatus paymentStatus;
    private int customerId;
    private int timeSlotId;
    private bool isSelected;
    private Customer? customer;     // null si constructeur int-IDs
    private TimeSlot? timeSlot;     // null pour les orders cart
    private Store? store;           // null pour les orders cart
    private List<OrderLine> orderLines = new List<OrderLine>();

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

    public int CustomerId
    {
        get => customer != null ? customer.UserId : customerId;
        set => customerId = value > 0
            ? value
            : throw new ArgumentException("CustomerId must be positive");
    }

    public int TimeSlotId
    {
        get => timeSlotId;
        set => timeSlotId = value > 0
            ? value
            : throw new ArgumentException("TimeSlotId must be positive");
    }

    public Customer? Customer
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

    // Constructeur kalou : navigation objects (utilisé par OrderDAL)
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
        customerId    = customer.UserId;
    }

    // Constructeur bywaa : int IDs (utilisé par les DAL de l'équipier)
    public Order(int orderId, DateTime orderDate, OrderStatus status,
                 int numberOfBoxes, int returnedBoxes, DateTime pickupDate,
                 PaymentStatus paymentStatus, int customerId, int timeSlotId)
    {
        OrderId       = orderId;
        OrderDate     = orderDate;
        Status        = status;
        NumberOfBoxes = numberOfBoxes;
        ReturnedBoxes = returnedBoxes;
        PickupDate    = pickupDate;
        PaymentStatus = paymentStatus;
        CustomerId    = customerId;
        TimeSlotId    = timeSlotId;
    }

    // ==================== STATIC ====================

    public static async Task<Order> GetByIdAsync(int orderId, IOrderDAL dal) =>
        await dal.GetById(orderId);

    public static async Task<List<Order>> GetOrdersByStoreAsync(int storeId, IOrderDAL dal) =>
        await dal.GetOrdersByStore(storeId);

    public static async Task<List<Order>> GetTodaysOrdersAsync(int storeId, IOrderDAL dal) =>
        await dal.GetTodayOrders(storeId);

    // ==================== INSTANCE ====================

    public float ComputeTotal(float productsTotal) =>
        productsTotal + SERVICE_FEE + (NumberOfBoxes * BOX_DEPOSIT) - (ReturnedBoxes * BOX_DEPOSIT);

    public async Task<float> ComputeTotalAsync(IOrderLineDAL dal)
    {
        List<OrderLine> lines = await dal.GetOrderLines(orderId);
        float productsTotal = lines.Sum(ol => ol.Product.Price * ol.Quantity);
        return ComputeTotal(productsTotal);
    }

    public Order GetSelected(bool selected)
    {
        isSelected = selected;
        return this;
    }

    public async Task<List<OrderLine>> GetOrderLinesAsync(IOrderLineDAL dal) =>
        await dal.GetOrderLines(orderId);

    public async Task AddProductAsync(int productId, IOrderLineDAL dal, int quantity = 1) =>
        await dal.AddProduct(orderId, productId, quantity);

    public async Task SetTimeSlotAsync(int timeSlotId, IOrderDAL dal) =>
        await dal.SetTimeSlot(orderId, timeSlotId);

    public void SetStore(int storeId)
    {
        if (store == null)
            store = new Store(storeId, "-", "-");
        else
            store.StoreId = storeId;
    }

    public async Task SetStatusAsync(OrderStatus status, IOrderDAL dal) =>
        await dal.SetStatus(orderId, status);

    public async Task SetNumberOfBoxesAsync(int numberOfBoxes, IOrderDAL dal) =>
        await dal.SetNumberOfBoxes(orderId, numberOfBoxes);

    public async Task SetReturnedBoxesAsync(int returnedBoxes, IOrderDAL dal) =>
        await dal.SetReturnedBoxes(orderId, returnedBoxes);

    public override string ToString() =>
        $"[Order] Id={OrderId} | Status={Status} | PickupDate={PickupDate:dd/MM/yyyy HH:mm} | CustomerId={CustomerId}";

    public override bool Equals(object obj)
    {
        if (obj is not Order other) return false;
        return OrderId == other.OrderId;
    }

    public override int GetHashCode() => OrderId.GetHashCode();
}

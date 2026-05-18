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
    private Customer customer;
    private TimeSlot? timeSlot;     // null pour les orders cart (pas encore de créneau)
    private Store? store;           // null pour les orders cart (pas de store lié)
    private List<OrderLine> orderLines;

    public int CustomerId => customer.UserId;
    private int timeSlotId;
    private bool isSelected;

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

    // ==================== STATIC ====================

    public static async Task<Order> GetByIdAsync(int orderId, IOrderDAL dal) => //
        await dal.GetById(orderId);

    // ==================== INSTANCE ====================

    public float ComputeTotal(float productsTotal) //
    {
        productsTotal += SERVICE_FEE + (NumberOfBoxes * BOX_DEPOSIT) - (ReturnedBoxes * BOX_DEPOSIT);
        return productsTotal;
    }

    public async Task SetTimeSlotAsync(int timeSlotId, IOrderDAL dal) => //
        await dal.SetTimeSlot(orderId, timeSlotId);

    public void SetStore(int storeId) //
    {
        if (store == null)
            store = new Store(storeId, "-", "-");
        else
            store.StoreId = storeId;
    }

    public async Task SetStatusAsync(OrderStatus status, IOrderDAL dal) => //
        await dal.SetStatus(orderId, status);

    public async Task SetNumberOfBoxesAsync(int numberOfBoxes, IOrderDAL dal) => //
        await dal.SetNumberOfBoxes(orderId, numberOfBoxes);

    public async Task SetReturnedBoxesAsync(int returnedBoxes, IOrderDAL dal) => //
        await dal.SetReturnedBoxes(orderId, returnedBoxes);

    public async Task<List<OrderLine>> GetOrderLinesAsync(IOrderLineDAL dal) => //
        await dal.GetOrderLines(orderId);

    public async Task AddProductAsync(int productId, IOrderLineDAL dal, int quantity = 1) => //
        await dal.AddProduct(orderId, productId, quantity);
}

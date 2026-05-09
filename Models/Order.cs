using ClickAndGoApp.Models.Enums;

namespace ClickAndGoApp.Models
{
    public class Order
    {
        private int orderId;
        private DateTime orderDate;
        private OrderStatus status;
        private int numberOfBoxes;
        private int returnedBoxes;
        private DateTime pickupDate;
        private PaymentStatus paymentStatus;
        private int customerId;
        private int timeSlotId;

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
            set => pickupDate = value != default
                ? value
                : throw new ArgumentException("PickupDate is not valid");
        }

        public PaymentStatus PaymentStatus
        {
            get => paymentStatus;
            set => paymentStatus = value;
        }

        public int CustomerId
        {
            get => customerId;
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

        public Order(int orderId, DateTime orderDate, OrderStatus status,
                     int numberOfBoxes, int returnedBoxes, DateTime pickupDate,
                     PaymentStatus paymentStatus, int customerId, int timeSlotId)
        {
            OrderId = orderId;
            OrderDate = orderDate;
            Status = status;
            NumberOfBoxes = numberOfBoxes;
            ReturnedBoxes = returnedBoxes;
            PickupDate = pickupDate;
            PaymentStatus = paymentStatus;
            CustomerId = customerId;
            TimeSlotId = timeSlotId;
        }

        public float ComputeTotal(float productsTotal)
        {
            return productsTotal
                   + SERVICE_FEE
                   + (NumberOfBoxes * BOX_DEPOSIT)
                   - (ReturnedBoxes * BOX_DEPOSIT);
        }
    }
}
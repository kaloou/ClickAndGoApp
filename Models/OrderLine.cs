namespace ClickAndCollect.Models
{
    public class OrderLine
    {
        private int orderId;
        private int productId;
        private int quantity;

        public int OrderId
        {
            get => orderId;
            set => orderId = value > 0
                ? value
                : throw new ArgumentException("OrderId must be positive");
        }

        public int ProductId
        {
            get => productId;
            set => productId = value > 0
                ? value
                : throw new ArgumentException("ProductId must be positive");
        }

        public int Quantity
        {
            get => quantity;
            set => quantity = value > 0
                ? value
                : throw new ArgumentException("Quantity must be positive");
        }

        public OrderLine(int orderId, int productId, int quantity)
        {
            OrderId = orderId;
            ProductId = productId;
            Quantity = quantity;
        }
    }
}
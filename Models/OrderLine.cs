namespace ClickAndGoApp.Models
{
    public class OrderLine
    {
        private int orderId;
        private int productId;
        private int quantity;
        private Product product;

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
        
        public Product Product
        {
            get { return product; }
            set => product = value ?? throw new ArgumentNullException("Product cannot be null");
        }

        public OrderLine(int orderId, int productId, int quantity, Product product)
        {
            OrderId = orderId;
            ProductId = productId;
            Quantity = quantity;
            Product = product;
        }
    }
}
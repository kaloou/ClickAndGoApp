using ClickAndGoApp.DAL.interfaces;

namespace ClickAndGoApp.Models
{
    public class OrderLine
    {
        private int orderId;
        private int quantity;
        private Product product;

        public int OrderId
        {
            get => orderId;
            set => orderId = value > 0
                ? value
                : throw new ArgumentException("OrderId must be positive");
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
            get => product;
            set => product = value ?? throw new ArgumentNullException("Product cannot be null");
        }

        public OrderLine(int orderId, int quantity, Product product)
        {
            OrderId  = orderId;
            Quantity = quantity;
            Product  = product;
        }

        //methodes

        public Product GetProduct() => product;   //

        public Task Remove(int productId, IOrderLineDAL dal) => //
            dal.Remove(orderId, productId);

        public Task SetQuantity(int productId, int quantity, IOrderLineDAL dal) => //
            dal.SetQuantity(orderId, productId, quantity);
    }
}

namespace ClickAndGoApp.Models
{
    public class Product
    {
        private int productId;
        private string name;
        private float price;
        private int categoryId;
        private string imagePath;
        private string description;

        public int ProductId
        {
            get => productId;
            set => productId = value > 0
                ? value
                : throw new ArgumentException("ProductId must be positive");
        }

        public string Name
        {
            get => name;
            set => name = !string.IsNullOrWhiteSpace(value)
                ? value
                : throw new ArgumentException("Name cannot be empty");
        }

        public float Price
        {
            get => price;
            set => price = value >= 0
                ? value
                : throw new ArgumentException("Price cannot be negative");
        }

        public int CategoryId
        {
            get => categoryId;
            set => categoryId = value > 0
                ? value
                : throw new ArgumentException("CategoryId must be positive");
        }

        public string ImagePath
        {
            get => imagePath;
            set => imagePath = value;
        }
        
        public string Description
        {
            get => description;
            set => description = value;
        }

        public Product(int productId, string name, float price,
            int categoryId, string description,string imagePath = null)
        {
            ProductId = productId;
            Name = name;
            Price = price;
            CategoryId = categoryId;
            Description = description;
            ImagePath = imagePath;
        }
    }
}
namespace ClickAndGoApp.Models
{
    public class Store
    {
        private int storeId;
        private string name;
        private string address;

        public int StoreId
        {
            get { return storeId; }
            set => storeId = value > 0
                ? value
                : throw new ArgumentException("StoreId must be positive");
        }

        public string Name
        {
            get { return name; }
            set => name = !string.IsNullOrWhiteSpace(value)
                ? value
                : throw new ArgumentException("Name cannot be empty");
        }

        public string Address
        {
            get { return address; }
            set => address = !string.IsNullOrWhiteSpace(value)
                ? value
                : throw new ArgumentException("Address cannot be empty");
        }

        public Store(int storeId, string name, string address)
        {
            StoreId = storeId;
            Name = name;
            Address = address;
        }
    }
}
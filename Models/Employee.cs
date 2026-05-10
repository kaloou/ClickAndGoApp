namespace ClickAndCollect.Models
{
    public class Employee : User
    {
        private int storeId;

        public int StoreId
        {
            get { return storeId; }
            set => storeId = value > 0
                ? value
                : throw new ArgumentException("StoreId must be positive");
        }

        public Employee(int userId, string firstName, string lastName,
            string email, string password, int storeId)
            : base(userId, firstName, lastName, email, password)
        {
            StoreId = storeId;
        }
    }
}
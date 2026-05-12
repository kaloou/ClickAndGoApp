namespace ClickAndGoApp.Models
{
    public class OrderPicker : Employee
    {
        public OrderPicker(int userId, string firstName, string lastName,
            string email, string password, int storeId)
            : base(userId, firstName, lastName, email, password, storeId)
        {
        }
    }
}
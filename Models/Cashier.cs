using ClickAndCollect.Models;

namespace ClickAndGoApp.Models
{
    public class Cashier : Employee
    {
        public Cashier(int userId, string firstName, string lastName,
            string email, string password, int storeId)
            : base(userId, firstName, lastName, email, password, storeId)
        {
        }
    }
}
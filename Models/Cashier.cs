using ClickAndGoApp.DAL.interfaces;

namespace ClickAndGoApp.Models
{
    public class Cashier : Employee
    {
        public Cashier(int userId, string firstName, string lastName,
            string email, string password, Store store)
            : base(userId, firstName, lastName, email, password, store)
        {
        }

        //methodes

        public static Task<Cashier> GetById(int cashierId, ICashierDAL dal) => //
            dal.GetById(cashierId);
    }
}

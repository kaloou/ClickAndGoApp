using ClickAndGoApp.DAL.interfaces;

namespace ClickAndGoApp.Models
{
    public class OrderPicker : Employee
    {
        public OrderPicker(int userId, string firstName, string lastName,
            string email, string password, Store store)
            : base(userId, firstName, lastName, email, password, store)
        {
        }

        //methodes

        public static Task<OrderPicker> GetById(int pickerId, IOrderPickerDAL dal) => //
            dal.GetById(pickerId);
    }
}

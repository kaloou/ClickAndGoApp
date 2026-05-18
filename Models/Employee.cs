using ClickAndGoApp.DAL.interfaces;

namespace ClickAndGoApp.Models
{
    public class Employee : User
    {
        private Store store;

        public Store Store
        {
            get => store;
            set => store = value;
        }

        public Employee(int userId, string firstName, string lastName,
            string email, string password, Store store)
            : base(userId, firstName, lastName, email, password)
        {
            Store = store;
        }

        //methodes
        //public Store GetStore() => store; //
    }
}

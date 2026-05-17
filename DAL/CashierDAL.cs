using ClickAndGoApp.Models;
using ClickAndGoApp.DAL.interfaces;

namespace ClickAndGoApp.DAL
{
    public class CashierDAL : ICashierDAL
    {
        private readonly DBConnection _db;

        public CashierDAL(DBConnection db)
        {
            _db = db;
        }

        public Task<Cashier> GetById(int cashierId) => Task.FromResult<Cashier>(null);
    }
}

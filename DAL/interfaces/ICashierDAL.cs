using ClickAndGoApp.Models;

namespace ClickAndGoApp.DAL.interfaces;

public interface ICashierDAL
{
    Task<Cashier> GetById(int cashierId);
}

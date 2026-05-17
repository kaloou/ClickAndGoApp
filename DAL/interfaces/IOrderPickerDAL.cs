using ClickAndGoApp.Models;

namespace ClickAndGoApp.DAL.interfaces;

public interface IOrderPickerDAL
{
    Task<OrderPicker> GetById(int pickerId);
}

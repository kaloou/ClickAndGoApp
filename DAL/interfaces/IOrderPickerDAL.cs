using ClickAndGoApp.Models;

namespace ClickAndGoApp.DAL;

public interface IOrderPickerDAL
{
    Task<OrderPicker> GetByIdAsync(int pickerId);
}

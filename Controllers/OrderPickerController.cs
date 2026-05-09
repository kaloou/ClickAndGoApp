using ClickAndCollect.DAL;
using Microsoft.AspNetCore.Mvc;
using ClickAndGoApp.DAL;
using ClickAndGoApp.Models;
//Je mets comm en fr mais je traduirai tout avant de rendre(plus simple)
namespace ClickAndCollect.Controllers
{
    public class OrderPickerController : Controller
    {
        private readonly OrderPickerDAL _orderPickerDAL;
        private readonly StoreDAL _storeDAL;
        private readonly OrderDAL _orderDAL;

        public OrderPickerController(OrderPickerDAL orderPickerDAL,
            StoreDAL storeDAL,
            OrderDAL orderDAL)
        {
            _orderPickerDAL = orderPickerDAL;
            _storeDAL = storeDAL;
            _orderDAL = orderDAL;
        }

        public IActionResult Index()
        {
            // Protection de la page
            if (HttpContext.Session.GetString("role") != "OrderPicker")
                return RedirectToAction("Login", "Auth");

            // Cascade du diagramme de séquence
            int pickerId = (int)HttpContext.Session.GetInt32("userId");
            OrderPicker orderPicker = _orderPickerDAL.GetById(pickerId);
            Store store = _storeDAL.GetStore(orderPicker.StoreId);
            List<Order> orders = _orderDAL.GetOrdersByStore(store.StoreId);

            // Alt : liste vide ou pas(diag de sequence)
            if (orders.Count == 0)
                ViewBag.Message = "No orders to prepare";

            return View(orders);
        }
    }
}
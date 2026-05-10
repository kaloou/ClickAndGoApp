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
        private readonly OrderLineDAL _orderLineDAL;

        public OrderPickerController(OrderPickerDAL orderPickerDAL,
            StoreDAL storeDAL,
            OrderDAL orderDAL,OrderLineDAL orderLineDAL)
        {
            _orderPickerDAL = orderPickerDAL;
            _storeDAL = storeDAL;
            _orderDAL = orderDAL;
            _orderLineDAL = orderLineDAL;

        }
        
        // ======= View Orders To Prepare ======= 
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
        
        // ====== View Order Details ====== 
        
        public IActionResult OrderDetails(int orderId)
        {
            if (HttpContext.Session.GetString("role") != "OrderPicker")
                return RedirectToAction("Login", "Auth");

            Order order = _orderDAL.GetById(orderId);

            if (order == null)
                return RedirectToAction("Index");

            List<OrderLine> orderLines = _orderLineDAL.GetOrderLines(orderId);

            ViewBag.Order = order;
            return View(orderLines);
        }
        
        // ============================================
// Encode Number of Boxes Used
// ============================================
        [HttpPost]
        public IActionResult EncodeBoxes(int orderId, int numberOfBoxes)
        {
            if (HttpContext.Session.GetString("role") != "OrderPicker")
                return RedirectToAction("Login", "Auth");

            if (numberOfBoxes <= 0)
            {
                Order order = _orderDAL.GetById(orderId);
                List<OrderLine> orderLines = _orderLineDAL.GetOrderLines(orderId);
                ViewBag.Order = order;
                ViewBag.Error = "Invalid value — number of boxes must be greater than 0";
                return View("OrderDetails", orderLines);
            }

            _orderDAL.SetNumberOfBoxes(orderId, numberOfBoxes);

            Order updatedOrder = _orderDAL.GetById(orderId);
            List<OrderLine> updatedOrderLines = _orderLineDAL.GetOrderLines(orderId);
            ViewBag.Order = updatedOrder;
            ViewBag.Success = "Number of boxes saved successfully";
            return View("OrderDetails", updatedOrderLines);
        }
    }
}
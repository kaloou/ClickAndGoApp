using Microsoft.AspNetCore.Mvc;
using ClickAndGoApp.DAL;
using ClickAndGoApp.Models;
using ClickAndGoApp.ViewModels;
using ClickAndGoApp.Exceptions;
using ClickAndGoApp.Models.Enums;
namespace ClickAndGoApp.Controllers
{
    public class OrderPickerController : Controller
    {
        private readonly IOrderPickerDAL _orderPickerDAL;
        private readonly IOrderDAL _orderDAL;
        private readonly IOrderLineDAL _orderLineDAL;

        public OrderPickerController(IOrderPickerDAL orderPickerDAL,
            IOrderDAL orderDAL,
            IOrderLineDAL orderLineDAL)
        {
            _orderPickerDAL = orderPickerDAL;
            _orderDAL = orderDAL;
            _orderLineDAL = orderLineDAL;
        }

        // ============================================
        // View Orders To Prepare
        // ============================================
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("role") != "OrderPicker")
                return RedirectToAction("Login", "Auth");

            ViewBag.FirstName = HttpContext.Session.GetString("firstName");

            try
            {
                int pickerId = (int)HttpContext.Session.GetInt32("userId");
                OrderPicker orderPicker = await OrderPicker.GetByIdAsync(pickerId, _orderPickerDAL);
                Store store = await orderPicker.GetStoreAsync();
                List<Order> orders = await store.GetOrdersByStoreAsync(_orderDAL);

                if (orders.Count == 0)
                    ViewBag.Message = "No orders to prepare";

                return View(orders);
            }
            catch (EntityNotFoundException)
            {
                return RedirectToAction("Login", "Auth");
            }
        }

        // ============================================
        // View Order Details
        // ============================================
        public async Task<IActionResult> OrderDetails(int id)
        {
            if (HttpContext.Session.GetString("role") != "OrderPicker")
                return RedirectToAction("Login", "Auth");

            ViewBag.FirstName = HttpContext.Session.GetString("firstName");

            try
            {
                Order order = await Order.GetByIdAsync(id, _orderDAL);
                List<OrderLine> orderLines = await order.GetOrderLinesAsync(_orderLineDAL);
                List<Product> products = orderLines.Select(ol => ol.GetProduct()).ToList();

                return View(new OrderDetailsViewModel(order, orderLines, products));
            }
            catch (EntityNotFoundException)
            {
                return RedirectToAction("Index");
            }
        }

        // ============================================
        // Encode Number of Boxes Used
        // ============================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EncodeBoxes(int orderId, int numberOfBoxes)
        {
            if (HttpContext.Session.GetString("role") != "OrderPicker")
                return RedirectToAction("Login", "Auth");

            ViewBag.FirstName = HttpContext.Session.GetString("firstName");

            try
            {
                Order order = await Order.GetByIdAsync(orderId, _orderDAL);
                List<OrderLine> orderLines = await order.GetOrderLinesAsync(_orderLineDAL);
                List<Product> products = orderLines.Select(ol => ol.GetProduct()).ToList();

                if (numberOfBoxes <= 0)
                {
                    ViewBag.Error = "Invalid value — number of boxes must be greater than 0";
                    return View("OrderDetails", new OrderDetailsViewModel(order, orderLines, products));
                }

                await order.SetNumberOfBoxesAsync(numberOfBoxes, _orderDAL);

                order = await Order.GetByIdAsync(orderId, _orderDAL);
                orderLines = await order.GetOrderLinesAsync(_orderLineDAL);
                products = orderLines.Select(ol => ol.GetProduct()).ToList();

                ViewBag.Success = "Number of boxes saved successfully";
                return View("OrderDetails", new OrderDetailsViewModel(order, orderLines, products));
            }
            catch (EntityNotFoundException)
            {
                return RedirectToAction("Index");
            }
        }

        // ============================================
        // Mark Order As Ready
        // ============================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsReady(int orderId)
        {
            if (HttpContext.Session.GetString("role") != "OrderPicker")
                return RedirectToAction("Login", "Auth");

            try
            {
                Order order = await Order.GetByIdAsync(orderId, _orderDAL);
                List<OrderLine> orderLines = await order.GetOrderLinesAsync(_orderLineDAL);
                List<Product> products = orderLines.Select(ol => ol.GetProduct()).ToList();

                if (order.NumberOfBoxes <= 0)
                {
                    ViewBag.Error = "Please encode the number of boxes before marking the order as ready.";
                    return View("OrderDetails", new OrderDetailsViewModel(order, orderLines, products));
                }

                await order.SetStatusAsync(OrderStatus.Ready, _orderDAL);
                TempData["Success"] = "Order is ready";
                return RedirectToAction("Index");
            }
            catch (EntityNotFoundException)
            {
                return RedirectToAction("Index");
            }
        }
    }
}

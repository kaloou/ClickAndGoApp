using Microsoft.AspNetCore.Mvc;
using ClickAndGoApp.DAL;
using ClickAndGoApp.Models;
using ClickAndGoApp.Models.Enums;
using ClickAndGoApp.ViewModels;

namespace ClickAndGoApp.Controllers
{
    public class CashierController : Controller
    {
        private readonly ICashierDAL _cashierDAL;
        private readonly IOrderDAL _orderDAL;
        private readonly IOrderLineDAL _orderLineDAL;

        public CashierController(ICashierDAL cashierDAL,
            IOrderDAL orderDAL,
            IOrderLineDAL orderLineDAL)
        {
            _cashierDAL = cashierDAL;
            _orderDAL = orderDAL;
            _orderLineDAL = orderLineDAL;
        }

        // ============================================
        // View Today's Orders
        // ============================================
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("role") != "Cashier")
                return RedirectToAction("Login", "Auth");

            ViewBag.FirstName = HttpContext.Session.GetString("firstName");

            int cashierId = (int)HttpContext.Session.GetInt32("userId");
            Cashier cashier = await Cashier.GetByIdAsync(cashierId, _cashierDAL);
            Store store = await cashier.GetStoreAsync();
            List<Order> orders = await store.GetTodaysOrdersAsync(_orderDAL);

            if (orders.Count == 0)
                ViewBag.Message = "No orders scheduled today";

            return View(orders);
        }

        // ============================================
        // Select Command
        // ============================================
        [HttpGet]
        public async Task<IActionResult> SelectCommand(int orderId)
        {
            if (HttpContext.Session.GetString("role") != "Cashier")
                return RedirectToAction("Login", "Auth");

            ViewBag.FirstName = HttpContext.Session.GetString("firstName");

            Order order = await Order.GetByIdAsync(orderId, _orderDAL);

            if (order == null)
                return RedirectToAction("Index");

            Order selectedOrder = order.GetSelected(true);

            return View(new CashierOrderViewModel(selectedOrder, await selectedOrder.ComputeTotalAsync(_orderLineDAL)));
        }

        // ============================================
        // Enter Number of Returned Boxes
        // ============================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnterReturnedBoxes(int orderId, int returnedBoxes)
        {
            if (HttpContext.Session.GetString("role") != "Cashier")
                return RedirectToAction("Login", "Auth");

            ViewBag.FirstName = HttpContext.Session.GetString("firstName");

            Order order = await Order.GetByIdAsync(orderId, _orderDAL);

            if (returnedBoxes < 0)
            {
                ViewBag.Error = "Invalid value — number of returned boxes cannot be negative";
                return View("SelectCommand", new CashierOrderViewModel(order, await order.ComputeTotalAsync(_orderLineDAL)));
            }

            await order.SetReturnedBoxesAsync(returnedBoxes, _orderDAL);

            order = await Order.GetByIdAsync(orderId, _orderDAL);
            ViewBag.Success = "Returned boxes saved successfully";
            return View("SelectCommand", new CashierOrderViewModel(order, await order.ComputeTotalAsync(_orderLineDAL)));
        }

        // ============================================
        // Mark Order as Collected
        // ============================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsCollected(int orderId)
        {
            if (HttpContext.Session.GetString("role") != "Cashier")
                return RedirectToAction("Login", "Auth");

            ViewBag.FirstName = HttpContext.Session.GetString("firstName");

            int cashierId = (int)HttpContext.Session.GetInt32("userId");
            Cashier cashier = await Cashier.GetByIdAsync(cashierId, _cashierDAL);
            Store store = await cashier.GetStoreAsync();
            List<Order> orders = await store.GetTodaysOrdersAsync(_orderDAL);

            Order order = await Order.GetByIdAsync(orderId, _orderDAL);
            if (order == null)
                return RedirectToAction("Index");

            await order.SetStatusAsync(OrderStatus.Honored, _orderDAL);
            orders.Remove(order);

            ViewBag.Success = "Order marked as collected successfully";
            return View("Index", orders);
        }
    }
}

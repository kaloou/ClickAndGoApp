using Microsoft.AspNetCore.Mvc;
using ClickAndGoApp.DAL;
using ClickAndGoApp.Models;
using ClickAndGoApp.Models.Enums;
using ClickAndGoApp.ViewModels;

namespace ClickAndGoApp.Controllers;

public class OrderController : Controller
{
    private readonly IOrderDAL     orderDal;
    private readonly IOrderLineDAL orderLineDal;
    private readonly IStoreDAL     storeDal;
    private readonly ITimeSlotDAL  timeSlotDal;

    public OrderController(IOrderDAL orderDal, IOrderLineDAL orderLineDal, IStoreDAL storeDal, ITimeSlotDAL timeSlotDal)
    {
        this.orderDal     = orderDal;
        this.orderLineDal = orderLineDal;
        this.storeDal     = storeDal;
        this.timeSlotDal  = timeSlotDal;
    }

    // ====== PlaceOrder ===== résumé panier + choix magasin et créneau 
    public async Task<IActionResult> Index()
    {
        int? userId = HttpContext.Session.GetInt32("userId");
        if (userId is null)
        {
            TempData["Error"] = "Vous n'êtes pas connecté";
            return RedirectToAction("Login", "Auth");
        }

        int? orderId = HttpContext.Session.GetInt32("orderId");
        if (orderId == null)
        {
            TempData["Error"] = "Pas de commande en cours";
            return RedirectToAction("Index", "Cart");
        }

        Order order = await Order.GetByIdAsync(orderId.Value, orderDal);
        if (order == null)
        {
            TempData["Error"] = "Commande introuvable";
            return RedirectToAction("Index", "Cart");
        }

        List<OrderLine> orderLines = await order.GetOrderLinesAsync(orderLineDal);
        if (!orderLines.Any())
        {
            TempData["Error"] = "Votre panier est vide.";
            return RedirectToAction("Index", "Cart");
        }

        // Adapter la vue selon si le store et crénéau son choisi
        int? selectedStoreId = HttpContext.Session.GetInt32("selectedStoreId");
        if (selectedStoreId != null)
        {
            Store store = await Store.GetStoreAsync(selectedStoreId.Value, storeDal);
            ViewBag.SelectedStore = store;
        }

        if (order.TimeSlotId != 0) // signifie que le timeSlot n'a pas encore été choisi
            ViewBag.Total = await order.ComputeTotalAsync(orderLineDal);
        
        var vm = new CartViewModel(order, orderLines);
        
        return View(vm);
    }

    // ====== SelectStore ======
    public async Task<IActionResult> SelectStore()
    {
        int? orderId = HttpContext.Session.GetInt32("orderId");
        if (orderId == null)
        {
            TempData["Error"] = "Pas de commande en cours";
            return RedirectToAction("Index", "Cart");
        }
        
        List<Store> stores = await Store.GetAllStoresAsync(storeDal);
        if (!stores.Any())
        {
            TempData["Error"] = "Pas de magasin disponibles";
            return RedirectToAction("Index", "Cart");
        }
        
        return View(stores);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SelectStore(int storeId)
    {
        HttpContext.Session.SetInt32("selectedStoreId", storeId);
        return RedirectToAction("SelectTimeSlot");
    }

    // ====== SelectTimeSlot ======
    public async Task<IActionResult> SelectTimeSlot(string selectedDate = null)
    {
        if (HttpContext.Session.GetInt32("orderId") == null)
            return RedirectToAction("Index", "Cart");

        int? storeId = HttpContext.Session.GetInt32("selectedStoreId");
        if (storeId == null)
        {
            TempData["Error"] = "Veuillez d'abord sélectionner un magasin.";
            return RedirectToAction("SelectStore");
        }

        Store store = await Store.GetStoreAsync(storeId.Value, storeDal);
        List<TimeSlot> allSlots = await store.GetAvailableTimeSlotsAsync(timeSlotDal);

        ViewBag.StoreName = store.Name;

        if (selectedDate != null && DateTime.TryParse(selectedDate, out DateTime parsedDate))
        {
            ViewBag.SelectedDate = selectedDate;
            return View(allSlots.Where(s => s.StartTime.Date == parsedDate.Date).ToList());
        }

        ViewBag.SelectedDate = null;
        return View(new List<TimeSlot>());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SelectTimeSlot(int timeSlotId)
    {
        int? orderId = HttpContext.Session.GetInt32("orderId");
        if (orderId == null)
            return RedirectToAction("Index", "Cart");

        Order order = await Order.GetByIdAsync(orderId.Value, orderDal);
        await order.SetTimeSlotAsync(timeSlotId, orderDal);
        return RedirectToAction("Index");
    }

    // ====== ConfirmOrder ======
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmOrder()
    {
        int? orderId = HttpContext.Session.GetInt32("orderId");
        if (orderId == null)
            return RedirectToAction("Index", "Cart");

        Order order = await Order.GetByIdAsync(orderId.Value, orderDal);

        if (order.TimeSlotId == 0)
        {
            TempData["Error"] = "Veuillez sélectionner un créneau horaire.";
            return RedirectToAction("Index");
        }

        await order.SetStatusAsync(OrderStatus.Pending, orderDal);

        int? storeId = HttpContext.Session.GetInt32("selectedStoreId");
        if (storeId != null)
        {
            Store store = await Store.GetStoreAsync(storeId.Value, storeDal);
            TempData["StoreName"] = store?.Name;
        }

        float total = await order.ComputeTotalAsync(orderLineDal);
        TempData["Total"]      = total.ToString("F2");
        TempData["PickupDate"] = order.PickupDate.ToString("dd/MM/yyyy HH:mm");

        HttpContext.Session.Remove("orderId");
        HttpContext.Session.Remove("selectedStoreId");

        return RedirectToAction("Confirmation", new { orderId = order.OrderId });
    }

    // ====== Confirmation ======
    public async Task<IActionResult> Confirmation(int orderId)
    {
        Order order = await Order.GetByIdAsync(orderId, orderDal);
        if (order == null)
            return RedirectToAction("Index", "Home");

        return View(order);
    }
}

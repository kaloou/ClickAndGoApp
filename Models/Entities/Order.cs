using System.Runtime.InteropServices.JavaScript;
using ClickAndGoApp.Models.Enums;

namespace ClickAndGoApp.Models;

public class Order
{
    private int orderId;
    private DateTime orderDate;
    private OrderStatus status;
    private int numberOfBoxes = 0;
    private int returnedBoxes;
    private DateTime pickupDate;
    private PaymentStatus paymentStatus;

    private const float SERVICE_FEE = 5.95f;
    private const float BOX_DEPOSIT = 5.95f;
}
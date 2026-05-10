using ClickAndGoApp.DAL;
using Microsoft.Data.SqlClient;
using ClickAndGoApp.Models;
using ClickAndGoApp.Models.Enums;

namespace ClickAndCollect.DAL
{
    public class OrderDAL
    {
        private readonly DBConnection _db;

        public OrderDAL(DBConnection db)
        {
            _db = db;
        }

        public List<Order> GetOrdersByStore(int storeId)
        {
            using SqlConnection conn = _db.GetConnexion();
            conn.Open();

            string query = @"
                SELECT o.orderId, o.orderDate, o.status, o.numberOfBoxes,
                       o.returnedBoxes, o.pickupDate, o.paymentStatus,
                       o.customerId, o.timeSlotId
                FROM [Order] o
                JOIN TimeSlot ts ON o.timeSlotId = ts.timeSlotId
                WHERE ts.storeId = @storeId
                AND CAST(o.pickupDate AS DATE) = CAST(DATEADD(day, 1, GETDATE()) AS DATE)
                AND o.status != 'Honnored'";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@storeId", storeId);

            using SqlDataReader reader = cmd.ExecuteReader();

            List<Order> orders = new List<Order>();
            while (reader.Read())
            {
                orders.Add(new Order(
                    (int)reader["orderId"],
                    (DateTime)reader["orderDate"],
                    Enum.Parse<OrderStatus>((string)reader["status"]),
                    (int)reader["numberOfBoxes"],
                    (int)reader["returnedBoxes"],
                    (DateTime)reader["pickupDate"],
                    Enum.Parse<PaymentStatus>((string)reader["paymentStatus"]),
                    (int)reader["customerId"],
                    (int)reader["timeSlotId"]
                ));
            }
            return orders;
        }
        
        public Order GetById(int orderId)
        {
            using SqlConnection conn = _db.GetConnexion();
            conn.Open();

            string query = @"
                SELECT orderId, orderDate, status, numberOfBoxes,
                returnedBoxes, pickupDate, paymentStatus,
                customerId, timeSlotId
                FROM [Order]
                WHERE orderId = @orderId";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@orderId", orderId);

            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new Order(
                    (int)reader["orderId"],
                    (DateTime)reader["orderDate"],
                    Enum.Parse<OrderStatus>((string)reader["status"]),
                    (int)reader["numberOfBoxes"],
                    (int)reader["returnedBoxes"],
                    (DateTime)reader["pickupDate"],
                    Enum.Parse<PaymentStatus>((string)reader["paymentStatus"]),
                    (int)reader["customerId"],
                    (int)reader["timeSlotId"]
                );
            }
            return null;
        }
        
        public void SetNumberOfBoxes(int orderId, int numberOfBoxes)
        {
            using SqlConnection conn = _db.GetConnexion();
            conn.Open();

            string query = @"
            UPDATE [Order]
            SET numberOfBoxes = @numberOfBoxes
            WHERE orderId = @orderId";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@orderId", orderId);
            cmd.Parameters.AddWithValue("@numberOfBoxes", numberOfBoxes);

            cmd.ExecuteNonQuery();
        }
    }
}
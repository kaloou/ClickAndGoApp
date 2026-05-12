using ClickAndGoApp.DAL;
using Microsoft.Data.SqlClient;
using ClickAndGoApp.Models;

namespace ClickAndGoApp.DAL
{
    public class OrderLineDAL
    {
        private readonly DBConnection _db;

        public OrderLineDAL(DBConnection db)
        {
            _db = db;
        }

        public List<OrderLine> GetOrderLines(int orderId)
        {
            using SqlConnection conn = _db.GetConnexion();
            conn.Open();

            string query = @"
                SELECT ol.orderId, ol.productId, ol.quantity,
                       p.name, p.price, p.imagePath, p.categoryId
                FROM OrderLine ol
                JOIN Product p ON ol.productId = p.productId
                WHERE ol.orderId = @orderId";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@orderId", orderId);

            using SqlDataReader reader = cmd.ExecuteReader();

            List<OrderLine> orderLines = new List<OrderLine>();
            while (reader.Read())
            {
                Product product = new Product(
                    (int)reader["productId"],
                    (string)reader["name"],
                    (float)(decimal)reader["price"],
                    (int)reader["categoryId"],
                    reader["imagePath"] == DBNull.Value ? null : (string)reader["imagePath"]
                );

                orderLines.Add(new OrderLine(
                    (int)reader["orderId"],
                    (int)reader["productId"],
                    (int)reader["quantity"],
                    product
                ));
            }
            return orderLines;
        }
    }
}
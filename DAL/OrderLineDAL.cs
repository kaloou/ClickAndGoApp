using ClickAndGoApp.DAL.interfaces;
using ClickAndGoApp.Models;
using Microsoft.Data.SqlClient;

namespace ClickAndGoApp.DAL;

public class OrderLineDAL : IOrderLineDAL
{
    private readonly DBConnection db;

    public OrderLineDAL(DBConnection db)
    {
        this.db = db;
    }

    public List<OrderLine> GetOrderLines(int orderId)
    {
        using (SqlConnection conn = db.GetConnexion())
        {
            conn.Open();

            string query = @"
                SELECT ol.orderId, ol.quantity,
                       p.productId, p.name, p.price, p.description, p.imagePath,
                       c.categoryId, c.name AS categoryName
                FROM OrderLine ol
                JOIN Product p  ON ol.productId  = p.productId
                JOIN Category c ON p.categoryId  = c.categoryId
                WHERE ol.orderId = @orderId";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@orderId", orderId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    List<OrderLine> orderLines = new List<OrderLine>();
                    while (reader.Read())
                        orderLines.Add(ReadOrderLine(reader));
                    return orderLines;
                }
            }
        }
    }

    public void AddProduct(int orderId, int productId, int quantity)
    {
        //
    }

    public void Remove(int orderId, int productId)
    {
        //
    }

    public void SetQuantity(int orderId, int productId, int quantity)
    {
        //
    }

    async Task<List<OrderLine>> IOrderLineDAL.GetOrderLines(int orderId)
    {
        using (SqlConnection conn = db.GetConnexion())
        {
            await conn.OpenAsync();

            string query = @"
                SELECT ol.orderId, ol.quantity,
                       p.productId, p.name, p.price, p.description, p.imagePath,
                       c.categoryId, c.name AS categoryName
                FROM OrderLine ol
                JOIN Product p  ON ol.productId  = p.productId
                JOIN Category c ON p.categoryId  = c.categoryId
                WHERE ol.orderId = @orderId";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@orderId", orderId);

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    List<OrderLine> orderLines = new List<OrderLine>();
                    while (await reader.ReadAsync())
                        orderLines.Add(ReadOrderLine(reader));
                    return orderLines;
                }
            }
        }
    }

    async Task IOrderLineDAL.AddProduct(int orderId, int productId, int quantity)
    {
        using SqlConnection conn = db.GetConnexion();
        await conn.OpenAsync();

        const string query = @"
            INSERT INTO OrderLine (orderId, productId, quantity)
            VALUES (@orderId, @productId, @quantity)";

        using SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@orderId",   orderId);
        cmd.Parameters.AddWithValue("@productId", productId);
        cmd.Parameters.AddWithValue("@quantity",  quantity);
        await cmd.ExecuteNonQueryAsync();
    }

    async Task IOrderLineDAL.Remove(int orderId, int productId)
    {
        using SqlConnection conn = db.GetConnexion();
        await conn.OpenAsync();

        const string query = @"
            DELETE FROM OrderLine
            WHERE orderId = @orderId AND productId = @productId";

        using SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@orderId",   orderId);
        cmd.Parameters.AddWithValue("@productId", productId);
        await cmd.ExecuteNonQueryAsync();
    }

    async Task IOrderLineDAL.SetQuantity(int orderId, int productId, int quantity)
    {
        using SqlConnection conn = db.GetConnexion();
        await conn.OpenAsync();

        const string query = @"
            UPDATE OrderLine SET quantity = @quantity
            WHERE orderId = @orderId AND productId = @productId";

        using SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@orderId",   orderId);
        cmd.Parameters.AddWithValue("@productId", productId);
        cmd.Parameters.AddWithValue("@quantity",  quantity);
        await cmd.ExecuteNonQueryAsync();
    }

    private static OrderLine ReadOrderLine(SqlDataReader reader)
    {
        var category = new Category(
            (int)reader["categoryId"],
            (string)reader["categoryName"]
        );

        var product = new Product(
            (int)reader["productId"],
            (string)reader["name"],
            (float)(decimal)reader["price"],
            category,
            reader["description"] == DBNull.Value ? null : (string)reader["description"],
            reader["imagePath"]   == DBNull.Value ? null : (string)reader["imagePath"]
        );

        return new OrderLine(
            (int)reader["orderId"],
            (int)reader["quantity"],
            product
        );
    }
}

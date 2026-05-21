using Microsoft.Data.SqlClient;
using ClickAndGoApp.Models;

namespace ClickAndGoApp.DAL;

public class OrderLineDAL : IOrderLineDAL
{
    private readonly DBConnection _db;

    public OrderLineDAL(DBConnection db)
    {
        _db = db;
    }

    // ISNULL(..., 0) handles empty carts — SUM over no rows returns NULL, not 0.
    // The result is cast from decimal to float to match the model's Price type.
    public async Task<float> GetProductsTotalAsync(int orderId)
    {
        using (SqlConnection conn = _db.GetConnexion())
        {
            await conn.OpenAsync();

            string query = @"
                SELECT ISNULL(SUM(ol.quantity * p.price), 0)
                FROM OrderLine ol
                JOIN Product p ON ol.productId = p.productId
                WHERE ol.orderId = @orderId";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@orderId", orderId);
                object result = await cmd.ExecuteScalarAsync();
                return (float)(decimal)result;
            }
        }
    }

    // Fetches all order lines with their full product and category info in one query.
    public async Task<List<OrderLine>> GetOrderLinesAsync(int orderId)
    {
        using (SqlConnection conn = _db.GetConnexion())
        {
            await conn.OpenAsync();

            string query = @"
                SELECT ol.orderId, ol.productId, ol.quantity,
                       p.name, p.price, p.description, p.imagePath,
                       c.categoryId, c.name AS categoryName
                FROM OrderLine ol
                JOIN Product  p ON ol.productId  = p.productId
                JOIN Category c ON p.categoryId  = c.categoryId
                WHERE ol.orderId = @orderId";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@orderId", orderId);

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    var orderLines = new List<OrderLine>();
                    while (await reader.ReadAsync())
                        orderLines.Add(ReadOrderLine(reader));
                    return orderLines;
                }
            }
        }
    }

    public async Task AddProductAsync(int orderId, int productId, int quantity)
    {
        using (SqlConnection conn = _db.GetConnexion())
        {
            await conn.OpenAsync();

            const string query = @"
                INSERT INTO OrderLine (orderId, productId, quantity)
                VALUES (@orderId, @productId, @quantity)";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@orderId",   orderId);
                cmd.Parameters.AddWithValue("@productId", productId);
                cmd.Parameters.AddWithValue("@quantity",  quantity);
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }

    public async Task RemoveAsync(int orderId, int productId)
    {
        using (SqlConnection conn = _db.GetConnexion())
        {
            await conn.OpenAsync();

            // The WHERE clause uses both columns of the composite primary key.
            const string query = @"
                DELETE FROM OrderLine
                WHERE orderId = @orderId AND productId = @productId";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@orderId",   orderId);
                cmd.Parameters.AddWithValue("@productId", productId);
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }

    public async Task SetQuantityAsync(int orderId, int productId, int quantity)
    {
        using (SqlConnection conn = _db.GetConnexion())
        {
            await conn.OpenAsync();

            const string query = @"
                UPDATE OrderLine SET quantity = @quantity
                WHERE orderId = @orderId AND productId = @productId";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@orderId",   orderId);
                cmd.Parameters.AddWithValue("@productId", productId);
                cmd.Parameters.AddWithValue("@quantity",  quantity);
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }

    // Extracted into a private helper to avoid duplicating the mapping logic.
    private static OrderLine ReadOrderLine(SqlDataReader reader)
    {
        var category = new Category(
            (int)reader["categoryId"],
            (string)reader["categoryName"]
        );

        var product = new Product(
            (int)reader["productId"],
            (string)reader["name"],
            (float)(decimal)reader["price"], // SQL decimal → C# float
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

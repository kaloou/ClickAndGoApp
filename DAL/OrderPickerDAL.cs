using Microsoft.Data.SqlClient;
using ClickAndGoApp.Models;

namespace ClickAndGoApp.DAL;

public class OrderPickerDAL : IOrderPickerDAL
{
    private readonly DBConnection _db;

    public OrderPickerDAL(DBConnection db)
    {
        _db = db;
    }

    public async Task<OrderPicker> GetByIdAsync(int pickerId)
    {
        using SqlConnection conn = _db.GetConnexion();
        await conn.OpenAsync();

        string query = @"
            SELECT u.userId, u.firstName, u.lastName,
                   u.email, u.password,
                   s.storeId, s.name AS storeName, s.address AS storeAddress
            FROM [User] u
            JOIN Employee    e  ON u.userId = e.userId
            JOIN OrderPicker op ON e.userId = op.userId
            JOIN Store       s  ON e.storeId = s.storeId
            WHERE op.userId = @pickerId";

        using SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@pickerId", pickerId);

        using SqlDataReader reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            var store = new Store(
                (int)reader["storeId"],
                (string)reader["storeName"],
                (string)reader["storeAddress"]
            );
            return new OrderPicker(
                (int)reader["userId"],
                (string)reader["firstName"],
                (string)reader["lastName"],
                (string)reader["email"],
                (string)reader["password"],
                store
            );
        }
        return null;
    }
}

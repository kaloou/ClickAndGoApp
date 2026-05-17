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
                   u.email, u.password, e.storeId
            FROM [User] u
            JOIN Employee    e  ON u.userId = e.userId
            JOIN OrderPicker op ON e.userId = op.userId
            WHERE op.userId = @pickerId";

        using SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@pickerId", pickerId);

        using SqlDataReader reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new OrderPicker(
                (int)reader["userId"],
                (string)reader["firstName"],
                (string)reader["lastName"],
                (string)reader["email"],
                (string)reader["password"],
                (int)reader["storeId"]
            );
        }
        return null;
    }
}

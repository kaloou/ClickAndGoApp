using Microsoft.Data.SqlClient;
using ClickAndGoApp.Models;
// namespace via ClickAndGoApp.DAL

namespace ClickAndGoApp.DAL;

public class CashierDAL : ICashierDAL
{
    private readonly DBConnection _db;

    public CashierDAL(DBConnection db)
    {
        _db = db;
    }

    public async Task<Cashier> GetByIdAsync(int cashierId)
    {
        using SqlConnection conn = _db.GetConnexion();
        await conn.OpenAsync();

        const string query = @"
            SELECT u.userId, u.firstName, u.lastName,
                   u.email, u.password, e.storeId
            FROM [User] u
            JOIN Employee e ON u.userId = e.userId
            JOIN Cashier  c ON e.userId = c.userId
            WHERE c.userId = @cashierId";

        using SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@cashierId", cashierId);

        using SqlDataReader reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) return null;

        return new Cashier(
            (int)reader["userId"],
            (string)reader["firstName"],
            (string)reader["lastName"],
            (string)reader["email"],
            (string)reader["password"],
            (int)reader["storeId"]
        );
    }
}

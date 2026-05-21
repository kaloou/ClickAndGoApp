using Microsoft.Data.SqlClient;
using ClickAndGoApp.Models;
using ClickAndGoApp.Exceptions;

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
        using (SqlConnection conn = _db.GetConnexion())
        {
            await conn.OpenAsync();

            const string query = @"
                SELECT u.userId, u.firstName, u.lastName,
                       u.email, u.password,
                       s.storeId, s.name AS storeName, s.address AS storeAddress
                FROM [User] u
                JOIN Employee e ON u.userId = e.userId
                JOIN Cashier  c ON e.userId = c.userId
                JOIN Store    s ON e.storeId = s.storeId
                WHERE c.userId = @cashierId";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@cashierId", cashierId);

                try
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (!await reader.ReadAsync())
                            throw new EntityNotFoundException("Cashier", cashierId);

                        var store = new Store(
                            (int)reader["storeId"],
                            (string)reader["storeName"],
                            (string)reader["storeAddress"]
                        );
                        return new Cashier(
                            (int)reader["userId"],
                            (string)reader["firstName"],
                            (string)reader["lastName"],
                            (string)reader["email"],
                            (string)reader["password"],
                            store
                        );
                    }
                }
                catch (SqlException ex)
                {
                    throw new DatabaseException("Failed to retrieve cashier.", ex);
                }
            }
        }
    }
}

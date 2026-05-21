using Microsoft.Data.SqlClient;
using ClickAndGoApp.Models;

namespace ClickAndGoApp.DAL;

public class UserDAL : IUserDAL
{
    private readonly DBConnection _db;

    public UserDAL(DBConnection db)
    {
        _db = db;
    }

    // Fetches a user by email + password and determines their role using a CASE WHEN expression.
    // We use LEFT JOINs on all role tables so the role can be determined in a single query
    // rather than running one query per role type.
    public async Task<User> GetByCredentialsAsync(string email, string password)
    {
        using (SqlConnection conn = _db.GetConnexion())
        {
            await conn.OpenAsync();

            // The CASE WHEN checks which sub-table the user appears in to derive their role.
            // OrderPicker is checked before Cashier because both inherit from Employee —
            string query = @"
                SELECT u.userId, u.firstName, u.lastName, u.email, u.password,
                    CASE
                        WHEN op.userId IS NOT NULL THEN 'OrderPicker'
                        WHEN c.userId  IS NOT NULL THEN 'Cashier'
                        WHEN cu.userId IS NOT NULL THEN 'Customer'
                    END AS role
                FROM [User] u
                LEFT JOIN Customer    cu ON u.userId = cu.userId
                LEFT JOIN Employee    e  ON u.userId = e.userId
                LEFT JOIN OrderPicker op ON e.userId = op.userId
                LEFT JOIN Cashier     c  ON e.userId = c.userId
                WHERE u.email = @email AND u.password = @password";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@email",    email);
                cmd.Parameters.AddWithValue("@password", password);

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new User(
                            (int)reader["userId"],
                            (string)reader["firstName"],
                            (string)reader["lastName"],
                            (string)reader["email"],
                            (string)reader["password"],
                            (string)reader["role"]
                        );
                    }
                    // Returns null when no matching user is found — the controller handles this case.
                    return null;
                }
            }
        }
    }
}

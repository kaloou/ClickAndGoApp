using Microsoft.Data.SqlClient;
using ClickAndGoApp.DAL.interfaces;
using ClickAndGoApp.Models;

namespace ClickAndGoApp.DAL;

public class UserDAL : IUserDAL
{
    private readonly DBConnection db;
    
    public UserDAL(DBConnection db)
    {
        this.db = db;
    }

    // mieux utiliser IUserDAl
    public User GetByCredentials(string email, string password)
    {
        using SqlConnection conn = db.GetConnexion();
        conn.Open();

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

        using SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@email", email);
        cmd.Parameters.AddWithValue("@password", password);

        using SqlDataReader reader = cmd.ExecuteReader();

        if (reader.Read())
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
        return null;
    }
    
    async Task<User> IUserDAL.GetByCredentials(string email, string password)
    {
        using SqlConnection conn = db.GetConnexion();
        await conn.OpenAsync();

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

        using SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@email", email);
        cmd.Parameters.AddWithValue("@password", password);

        using SqlDataReader reader = await cmd.ExecuteReaderAsync();

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
        return null;
    }
}

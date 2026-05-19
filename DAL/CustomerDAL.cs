using Microsoft.Data.SqlClient;
using ClickAndGoApp.Models;
using ClickAndGoApp.Exceptions;

namespace ClickAndGoApp.DAL;

public class CustomerDAL : ICustomerDAL
{
    private readonly DBConnection db;

    public CustomerDAL(DBConnection db)
    {
        this.db = db;
    }

    public async Task<bool> GetByEmailAsync(string email)
    {
        using SqlConnection conn = db.GetConnexion();
        await conn.OpenAsync();

        const string query = "SELECT COUNT(*) FROM [User] WHERE email = @email";
        using SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@email", email);

        return (int)await cmd.ExecuteScalarAsync() > 0;
    }

    public async Task<Customer> CreateAccountAsync(string firstName, string lastName, string email, string password, string? phoneNumber, string? address)
    {
        using SqlConnection conn = db.GetConnexion();
        await conn.OpenAsync();
        using SqlTransaction tx = conn.BeginTransaction();

        const string insertUser = @"
            INSERT INTO [User] (firstName, lastName, email, password)
            OUTPUT INSERTED.userId
            VALUES (@firstName, @lastName, @email, @password)";

        using SqlCommand cmdUser = new SqlCommand(insertUser, conn, tx);
        cmdUser.Parameters.AddWithValue("@firstName", firstName);
        cmdUser.Parameters.AddWithValue("@lastName",  lastName);
        cmdUser.Parameters.AddWithValue("@email",     email);
        cmdUser.Parameters.AddWithValue("@password",  password);
        int userId = (int)await cmdUser.ExecuteScalarAsync();

        const string insertCustomer = @"
            INSERT INTO Customer (userId, loyaltyPoints, phoneNumber, address)
            VALUES (@userId, 0, @phoneNumber, @address)";

        using SqlCommand cmdCust = new SqlCommand(insertCustomer, conn, tx);
        cmdCust.Parameters.AddWithValue("@userId",      userId);
        cmdCust.Parameters.AddWithValue("@phoneNumber", (object?)phoneNumber ?? DBNull.Value);
        cmdCust.Parameters.AddWithValue("@address",     (object?)address     ?? DBNull.Value);
        await cmdCust.ExecuteNonQueryAsync();

        await tx.CommitAsync();

        int phone = int.TryParse(phoneNumber, out int p) ? p : 0;
        return new Customer(userId, firstName, lastName, email, password, 0, phone, address);
    }

    public async Task<Customer> GetByIdAsync(int userId)
    {
        using SqlConnection conn = db.GetConnexion();
        await conn.OpenAsync();

        const string query = @"
            SELECT u.userId, u.firstName, u.lastName, u.email, u.password,
                   c.loyaltyPoints, c.phoneNumber, c.address
            FROM [User] u
            JOIN Customer c ON u.userId = c.userId
            WHERE u.userId = @userId";

        using SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@userId", userId);

        try
        {
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return new Customer(
                    (int)reader["userId"],
                    (string)reader["firstName"],
                    (string)reader["lastName"],
                    (string)reader["email"],
                    (string)reader["password"],
                    (int)reader["loyaltyPoints"],
                    Convert.ToInt32(reader["phoneNumber"]),
                    reader["address"] == DBNull.Value ? null : (string)reader["address"]
                );
            throw new EntityNotFoundException("Customer", userId);
        }
        catch (SqlException ex)
        {
            throw new DatabaseException("Failed to retrieve customer.", ex);
        }
    }

    public async Task UpdateAsync(int userId, string firstName, string lastName, int phoneNumber, string? address, string password)
    {
        using SqlConnection conn = db.GetConnexion();
        await conn.OpenAsync();
        using SqlTransaction tx = conn.BeginTransaction();

        const string updateUser = @"
            UPDATE [User]
            SET firstName = @firstName, lastName = @lastName, password = @password
            WHERE userId = @userId";

        using SqlCommand cmdUser = new SqlCommand(updateUser, conn, tx);
        cmdUser.Parameters.AddWithValue("@userId",    userId);
        cmdUser.Parameters.AddWithValue("@firstName", firstName);
        cmdUser.Parameters.AddWithValue("@lastName",  lastName);
        cmdUser.Parameters.AddWithValue("@password",  password);
        await cmdUser.ExecuteNonQueryAsync();

        const string updateCustomer = @"
            UPDATE Customer
            SET phoneNumber = @phoneNumber, address = @address
            WHERE userId = @userId";

        using SqlCommand cmdCust = new SqlCommand(updateCustomer, conn, tx);
        cmdCust.Parameters.AddWithValue("@userId",      userId);
        cmdCust.Parameters.AddWithValue("@phoneNumber", phoneNumber);
        cmdCust.Parameters.AddWithValue("@address",     (object?)address ?? DBNull.Value);
        await cmdCust.ExecuteNonQueryAsync();

        try
        {
            await tx.CommitAsync();
        }
        catch (SqlException ex)
        {
            throw new DatabaseException("Failed to update customer.", ex);
        }
    }
}

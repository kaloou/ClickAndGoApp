using Microsoft.Data.SqlClient;
using ClickAndGoApp.DAL.interfaces;
using ClickAndGoApp.Models;

namespace ClickAndGoApp.DAL;

public class CustomerDAL : ICustomerDAL
{
    private readonly DBConnection db;

    public CustomerDAL(DBConnection db)
    {
        this.db = db;
    }

    public async Task<bool> GetByEmail(string email)
    {
        using SqlConnection conn = db.GetConnexion();
        await conn.OpenAsync();

        const string query = "SELECT COUNT(*) FROM [User] WHERE email = @email";
        using SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@email", email);

        return (int)await cmd.ExecuteScalarAsync() > 0;
    }

    public async Task<Customer> CreateAccount(string firstName, string lastName, string email, string password, string? phoneNumber, string? address)
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
}

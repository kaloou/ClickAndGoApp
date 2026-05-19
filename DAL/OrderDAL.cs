using Microsoft.Data.SqlClient;
using ClickAndGoApp.Models;
using ClickAndGoApp.Models.Enums;

namespace ClickAndGoApp.DAL;

public class OrderDAL : IOrderDAL
{
    private readonly DBConnection _db;

    public OrderDAL(DBConnection db)
    {
        _db = db;
    }

    public async Task<Order> GetByIdAsync(int orderId)
    {
        using SqlConnection conn = _db.GetConnexion();
        await conn.OpenAsync();

        string query = @"
            SELECT orderId, orderDate, status, numberOfBoxes,
                   returnedBoxes, pickupDate, paymentStatus,
                   customerId, timeSlotId
            FROM [Order]
            WHERE orderId = @orderId";

        using SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@orderId", orderId);

        using SqlDataReader reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
            return ReadOrder(reader);

        return null;
    }

    public async Task<List<Order>> GetOrdersByStoreAsync(int storeId)
    {
        using SqlConnection conn = _db.GetConnexion();
        await conn.OpenAsync();

        string query = @"
            SELECT o.orderId, o.orderDate, o.status, o.numberOfBoxes,
                   o.returnedBoxes, o.pickupDate, o.paymentStatus,
                   o.customerId, o.timeSlotId
            FROM [Order] o
            JOIN TimeSlot ts ON o.timeSlotId = ts.timeSlotId
            WHERE ts.storeId = @storeId
              AND CAST(o.pickupDate AS DATE) = CAST(DATEADD(day, 1, GETDATE()) AS DATE)
              AND o.status != 'Honored'";

        using SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@storeId", storeId);

        using SqlDataReader reader = await cmd.ExecuteReaderAsync();
        var orders = new List<Order>();
        while (await reader.ReadAsync())
            orders.Add(ReadOrder(reader));
        return orders;
    }

    public async Task<List<Order>> GetTodaysOrdersAsync(int storeId)
    {
        using SqlConnection conn = _db.GetConnexion();
        await conn.OpenAsync();

        string query = @"
            SELECT o.orderId, o.orderDate, o.status, o.numberOfBoxes,
                   o.returnedBoxes, o.pickupDate, o.paymentStatus,
                   o.customerId, o.timeSlotId
            FROM [Order] o
            JOIN TimeSlot ts ON o.timeSlotId = ts.timeSlotId
            WHERE ts.storeId = @storeId
              AND CAST(o.pickupDate AS DATE) = CAST(GETDATE() AS DATE)
              AND o.status != 'Honored'";

        using SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@storeId", storeId);

        using SqlDataReader reader = await cmd.ExecuteReaderAsync();
        var orders = new List<Order>();
        while (await reader.ReadAsync())
            orders.Add(ReadOrder(reader));
        return orders;
    }

    public async Task SetNumberOfBoxesAsync(int orderId, int numberOfBoxes)
    {
        using SqlConnection conn = _db.GetConnexion();
        await conn.OpenAsync();

        string query = "UPDATE [Order] SET numberOfBoxes = @numberOfBoxes WHERE orderId = @orderId";

        using SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@orderId", orderId);
        cmd.Parameters.AddWithValue("@numberOfBoxes", numberOfBoxes);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task SetReturnedBoxesAsync(int orderId, int returnedBoxes)
    {
        using SqlConnection conn = _db.GetConnexion();
        await conn.OpenAsync();

        string query = "UPDATE [Order] SET returnedBoxes = @returnedBoxes WHERE orderId = @orderId";

        using SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@orderId", orderId);
        cmd.Parameters.AddWithValue("@returnedBoxes", returnedBoxes);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task SetStatusAsync(int orderId, OrderStatus status)
    {
        using SqlConnection conn = _db.GetConnexion();
        await conn.OpenAsync();

        string query = "UPDATE [Order] SET status = @status WHERE orderId = @orderId";

        using SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@orderId", orderId);
        cmd.Parameters.AddWithValue("@status", status.ToString());
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<int> CreateOrderAsync(int customerId)
    {
        using SqlConnection conn = _db.GetConnexion();
        await conn.OpenAsync();

        const string query = @"
            INSERT INTO [Order] (customerId, status)
            OUTPUT INSERTED.orderId
            VALUES (@customerId, 'InTheCart')";

        using SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@customerId", customerId);
        return (int)(await cmd.ExecuteScalarAsync())!;
    }

    public Task<List<Order>> GetOrdersByCustomerAsync(int customerId) => Task.FromResult(new List<Order>());
    public Task<List<Order>> GetOrdersToPrepareAsync(int storeId) => Task.FromResult(new List<Order>());

    public async Task SetTimeSlotAsync(int orderId, int timeSlotId)
    {
        using SqlConnection conn = _db.GetConnexion();
        await conn.OpenAsync();

        string query = @"
            UPDATE [Order]
            SET timeSlotId = @timeSlotId,
                pickupDate = (SELECT startTime FROM TimeSlot WHERE timeSlotId = @timeSlotId)
            WHERE orderId = @orderId";

        using SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@orderId", orderId);
        cmd.Parameters.AddWithValue("@timeSlotId", timeSlotId);
        await cmd.ExecuteNonQueryAsync();
    }

    private static Order ReadOrder(SqlDataReader reader)
    {
        DateTime pickupDate = reader["pickupDate"] == DBNull.Value
            ? DateTime.MinValue
            : (DateTime)reader["pickupDate"];

        int timeSlotId = reader["timeSlotId"] == DBNull.Value ? 0 : (int)reader["timeSlotId"];

        return new Order(
            (int)reader["orderId"],
            (DateTime)reader["orderDate"],
            Enum.Parse<OrderStatus>((string)reader["status"]),
            (int)reader["numberOfBoxes"],
            (int)reader["returnedBoxes"],
            pickupDate,
            Enum.Parse<PaymentStatus>((string)reader["paymentStatus"]),
            (int)reader["customerId"],
            timeSlotId
        );
    }
}

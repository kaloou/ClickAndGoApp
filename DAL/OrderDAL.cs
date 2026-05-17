using ClickAndGoApp.DAL.interfaces;
using ClickAndGoApp.Models;
using ClickAndGoApp.Models.Enums;
using Microsoft.Data.SqlClient;

namespace ClickAndGoApp.DAL;

public class OrderDAL : IOrderDAL
{
    private readonly DBConnection db;

    public OrderDAL(DBConnection db)
    {
        this.db = db;
    }

    // LEFT JOIN : gère les orders cart et les orders confirmés
    private static readonly string BaseQuery = @"
        SELECT o.orderId, o.orderDate, o.status, o.numberOfBoxes,
               o.returnedBoxes, o.pickupDate, o.paymentStatus,
               ts.timeSlotId, ts.startTime, ts.endTime,
               s.storeId, s.name AS storeName, s.address AS storeAddress,
               u.userId, u.firstName, u.lastName, u.email, u.password,
               cu.loyaltyPoints, cu.phoneNumber, cu.address
        FROM [Order] o
        LEFT JOIN TimeSlot ts ON o.timeSlotId = ts.timeSlotId
        LEFT JOIN Store    s  ON ts.storeId   = s.storeId
        JOIN [User]   u  ON o.customerId  = u.userId
        JOIN Customer cu ON u.userId      = cu.userId";

    private static Order ReadOrder(SqlDataReader reader)
    {
        // null si order cart
        TimeSlot? timeSlot = reader["timeSlotId"] == DBNull.Value ? null : new TimeSlot(
            (int)reader["timeSlotId"],
            (DateTime)reader["startTime"],
            (DateTime)reader["endTime"]
        );

        Store? store = reader["storeId"] == DBNull.Value ? null : new Store(
            (int)reader["storeId"],
            (string)reader["storeName"],
            (string)reader["storeAddress"]
        );

        var customer = new Customer(
            (int)reader["userId"],
            (string)reader["firstName"],
            (string)reader["lastName"],
            (string)reader["email"],
            (string)reader["password"],
            (int)reader["loyaltyPoints"],
            reader["phoneNumber"] == DBNull.Value ? 0 : Convert.ToInt32(reader["phoneNumber"]),
            reader["address"] == DBNull.Value ? null : (string)reader["address"]
        );
        
        DateTime pickupDate = reader["pickupDate"] == DBNull.Value
            ? DateTime.MinValue
            : (DateTime)reader["pickupDate"];

        return new Order(
            (int)reader["orderId"],
            (DateTime)reader["orderDate"],
            Enum.Parse<OrderStatus>((string)reader["status"]),
            (int)reader["numberOfBoxes"],
            (int)reader["returnedBoxes"],
            pickupDate,
            Enum.Parse<PaymentStatus>((string)reader["paymentStatus"]),
            customer,
            timeSlot,
            store
        );
    }

    public int CreateOrder(int customerId)
    {
        using SqlConnection conn = db.GetConnexion();
        conn.Open();

        const string query = @"INSERT INTO [Order] (customerId, status)
                               OUTPUT INSERTED.orderId
                               VALUES (@customerId, 'InTheCart')";

        using SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@customerId", customerId);
        return (int)cmd.ExecuteScalar();
    }

    public Order GetById(int orderId)
    {
        using SqlConnection conn = db.GetConnexion();
        conn.Open();

        string query = BaseQuery + " WHERE o.orderId = @orderId";

        using SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@orderId", orderId);

        using SqlDataReader reader = cmd.ExecuteReader();
        return reader.Read() ? ReadOrder(reader) : null;
    }

    public List<Order> GetOrdersByStore(int storeId)
    {
        using SqlConnection conn = db.GetConnexion();
        conn.Open();

        // ts.storeId filtre naturellement les order cart
        string query = BaseQuery + @"
            WHERE ts.storeId = @storeId
              AND CAST(o.pickupDate AS DATE) = CAST(DATEADD(day, 1, GETDATE()) AS DATE)
              AND o.status != 'Honored'";

        using SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@storeId", storeId);

        using SqlDataReader reader = cmd.ExecuteReader();
        var orders = new List<Order>();
        while (reader.Read()) orders.Add(ReadOrder(reader));
        return orders;
    }

    public void SetNumberOfBoxes(int orderId, int numberOfBoxes)
    {
        using SqlConnection conn = db.GetConnexion();
        conn.Open();

        string query = "UPDATE [Order] SET numberOfBoxes = @numberOfBoxes WHERE orderId = @orderId";

        using SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@orderId", orderId);
        cmd.Parameters.AddWithValue("@numberOfBoxes", numberOfBoxes);
        cmd.ExecuteNonQuery();
    }

    public List<Order> GetOrdersByCustomer(int customerId) => null;
    public List<Order> GetOrdersToPrepare(int storeId) => null;
    public List<Order> GetTodayOrders(int storeId) => null;

    public void SetTimeSlot(int orderId, int tsId)
    {
        //
    }

    public void SetStatus(int orderId, OrderStatus s)
    {
        //
    }

    public void SetReturnedBoxes(int orderId, int n)
    {
        //
    }
    
    async Task<int> IOrderDAL.CreateOrder(int customerId)
    {
        using SqlConnection conn = db.GetConnexion();
        await conn.OpenAsync();

        const string query = @"INSERT INTO [Order] (customerId, status)
                               OUTPUT INSERTED.orderId
                               VALUES (@customerId, 'InTheCart')";

        using SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@customerId", customerId);
        return (int)(await cmd.ExecuteScalarAsync())!;
    }

    async Task<Order> IOrderDAL.GetById(int orderId)
    {
        using SqlConnection conn = db.GetConnexion();
        await conn.OpenAsync();

        string query = BaseQuery + " WHERE o.orderId = @orderId";

        using SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@orderId", orderId);

        using SqlDataReader reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync() ? ReadOrder(reader) : null;
    }

    async Task<List<Order>> IOrderDAL.GetOrdersByStore(int storeId)
    {
        using SqlConnection conn = db.GetConnexion();
        await conn.OpenAsync();

        string query = BaseQuery + @"
            WHERE ts.storeId = @storeId
              AND CAST(o.pickupDate AS DATE) = CAST(DATEADD(day, 1, GETDATE()) AS DATE)
              AND o.status != 'Honored'";

        using SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@storeId", storeId);

        using SqlDataReader reader = await cmd.ExecuteReaderAsync();
        var orders = new List<Order>();
        while (await reader.ReadAsync()) orders.Add(ReadOrder(reader));
        return orders;
    }

    async Task IOrderDAL.SetNumberOfBoxes(int orderId, int numberOfBoxes)
    {
        using SqlConnection conn = db.GetConnexion();
        await conn.OpenAsync();

        string query = "UPDATE [Order] SET numberOfBoxes = @numberOfBoxes WHERE orderId = @orderId";

        using SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@orderId", orderId);
        cmd.Parameters.AddWithValue("@numberOfBoxes", numberOfBoxes);
        await cmd.ExecuteNonQueryAsync();
    }

    Task<List<Order>> IOrderDAL.GetOrdersByCustomer(int customerId) => Task.FromResult<List<Order>>(null);
    Task<List<Order>> IOrderDAL.GetOrdersToPrepare(int storeId) => Task.FromResult<List<Order>>(null);
    Task<List<Order>> IOrderDAL.GetTodayOrders(int storeId) => Task.FromResult<List<Order>>(null);
    Task IOrderDAL.SetTimeSlot(int orderId, int timeSlotId) => Task.CompletedTask;
    Task IOrderDAL.SetStatus(int orderId, OrderStatus status) => Task.CompletedTask;
    Task IOrderDAL.SetReturnedBoxes(int orderId, int returnedBoxes) => Task.CompletedTask;
}

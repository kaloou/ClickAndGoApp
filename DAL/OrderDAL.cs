using Microsoft.Data.SqlClient;
using ClickAndGoApp.Models;
using ClickAndGoApp.Models.Enums;
using ClickAndGoApp.Exceptions;

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
        using (SqlConnection conn = _db.GetConnexion())
        {
            await conn.OpenAsync();

            // LEFT JOIN on TimeSlot because an order in the cart has no time slot yet.
            string query = @"
                SELECT o.orderId, o.orderDate, o.status, o.numberOfBoxes,
                       o.returnedBoxes, o.pickupDate, o.paymentStatus,
                       u.userId, u.firstName, u.lastName, u.email, u.password,
                       c.loyaltyPoints, c.phoneNumber, c.address,
                       ts.timeSlotId AS tsId, ts.startTime, ts.endTime
                FROM [Order] o
                JOIN [User]   u  ON o.customerId   = u.userId
                JOIN Customer c  ON u.userId        = c.userId
                LEFT JOIN TimeSlot ts ON o.timeSlotId = ts.timeSlotId
                WHERE o.orderId = @orderId";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@orderId", orderId);

                try
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                            return ReadOrder(reader);
                        throw new EntityNotFoundException("Order", orderId);
                    }
                }
                catch (SqlException ex)
                {
                    throw new DatabaseException("Failed to retrieve order.", ex);
                }
            }
        }
    }

    // Returns tomorrow's orders for the order picker — filtered by date and excluding
    // carts (InTheCart) and already-processed orders (Honored).
    public async Task<List<Order>> GetOrdersByStoreAsync(int storeId)
    {
        using (SqlConnection conn = _db.GetConnexion())
        {
            await conn.OpenAsync();

            string query = @"
                SELECT o.orderId, o.orderDate, o.status, o.numberOfBoxes,
                       o.returnedBoxes, o.pickupDate, o.paymentStatus,
                       u.userId, u.firstName, u.lastName, u.email, u.password,
                       c.loyaltyPoints, c.phoneNumber, c.address,
                       ts.timeSlotId AS tsId, ts.startTime, ts.endTime
                FROM [Order] o
                JOIN [User]   u  ON o.customerId   = u.userId
                JOIN Customer c  ON u.userId        = c.userId
                JOIN TimeSlot ts ON o.timeSlotId   = ts.timeSlotId
                WHERE ts.storeId = @storeId
                  AND CAST(o.pickupDate AS DATE) = CAST(DATEADD(day, 1, GETDATE()) AS DATE)
                  AND o.status NOT IN ('InTheCart', 'Honored')";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@storeId", storeId);

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    var orders = new List<Order>();
                    while (await reader.ReadAsync())
                    {
                        Order order = ReadOrder(reader);
                        order.SetStore(storeId);
                        orders.Add(order);
                    }
                    return orders;
                }
            }
        }
    }

    // Returns today's orders for the cashier — all statuses except Honored (already collected).
    public async Task<List<Order>> GetTodaysOrdersAsync(int storeId)
    {
        using (SqlConnection conn = _db.GetConnexion())
        {
            await conn.OpenAsync();

            string query = @"
                SELECT o.orderId, o.orderDate, o.status, o.numberOfBoxes,
                       o.returnedBoxes, o.pickupDate, o.paymentStatus,
                       u.userId, u.firstName, u.lastName, u.email, u.password,
                       c.loyaltyPoints, c.phoneNumber, c.address,
                       ts.timeSlotId AS tsId, ts.startTime, ts.endTime
                FROM [Order] o
                JOIN [User]   u  ON o.customerId   = u.userId
                JOIN Customer c  ON u.userId        = c.userId
                JOIN TimeSlot ts ON o.timeSlotId   = ts.timeSlotId
                WHERE ts.storeId = @storeId
                  AND CAST(o.pickupDate AS DATE) = CAST(GETDATE() AS DATE)
                  AND o.status != 'Honored'";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@storeId", storeId);

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    var orders = new List<Order>();
                    while (await reader.ReadAsync())
                    {
                        Order order = ReadOrder(reader);
                        order.SetStore(storeId);
                        orders.Add(order);
                    }
                    return orders;
                }
            }
        }
    }

    public async Task SetNumberOfBoxesAsync(int orderId, int numberOfBoxes)
    {
        using (SqlConnection conn = _db.GetConnexion())
        {
            await conn.OpenAsync();

            string query = "UPDATE [Order] SET numberOfBoxes = @numberOfBoxes WHERE orderId = @orderId";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@orderId",       orderId);
                cmd.Parameters.AddWithValue("@numberOfBoxes", numberOfBoxes);
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }

    public async Task SetReturnedBoxesAsync(int orderId, int returnedBoxes)
    {
        using (SqlConnection conn = _db.GetConnexion())
        {
            await conn.OpenAsync();

            string query = "UPDATE [Order] SET returnedBoxes = @returnedBoxes WHERE orderId = @orderId";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@orderId",       orderId);
                cmd.Parameters.AddWithValue("@returnedBoxes", returnedBoxes);
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }

    public async Task SetStatusAsync(int orderId, OrderStatus status)
    {
        using (SqlConnection conn = _db.GetConnexion())
        {
            await conn.OpenAsync();

            string query = "UPDATE [Order] SET status = @status WHERE orderId = @orderId";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@orderId", orderId);
                // Enum is stored as a string in the DB — ToString() converts e.g. OrderStatus.Ready → "Ready".
                cmd.Parameters.AddWithValue("@status",  status.ToString());
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }

    public async Task<int> CreateOrderAsync(int customerId)
    {
        using (SqlConnection conn = _db.GetConnexion())
        {
            await conn.OpenAsync();

            // OUTPUT INSERTED returns the generated orderId without a second SELECT call.
            const string query = @"
                INSERT INTO [Order] (customerId, status)
                OUTPUT INSERTED.orderId
                VALUES (@customerId, 'InTheCart')";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@customerId", customerId);
                return (int)(await cmd.ExecuteScalarAsync())!;
            }
        }
    }

    // Checks whether the customer already has an active cart so we don't create duplicate carts.
    // TOP 1 + ORDER BY DESC picks the most recent one in case of data inconsistency.
    // The ORDER BY must stay in SQL — it limits the result set before it's returned.
    public async Task<int?> GetActiveCartAsync(int customerId)
    {
        using (SqlConnection conn = _db.GetConnexion())
        {
            await conn.OpenAsync();

            const string query = @"
                SELECT TOP 1 orderId FROM [Order]
                WHERE customerId = @customerId AND status = 'InTheCart'
                ORDER BY orderId DESC";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@customerId", customerId);
                object? result = await cmd.ExecuteScalarAsync();
                return result is null ? null : (int)result;
            }
        }
    }

    // Returns the customer's order history — excludes carts (InTheCart) which are not confirmed orders.
    public async Task<List<Order>> GetOrdersByCustomerAsync(int customerId)
    {
        using (SqlConnection conn = _db.GetConnexion())
        {
            await conn.OpenAsync();

            string query = @"
                SELECT o.orderId, o.orderDate, o.status, o.numberOfBoxes,
                       o.returnedBoxes, o.pickupDate, o.paymentStatus,
                       u.userId, u.firstName, u.lastName, u.email, u.password,
                       c.loyaltyPoints, c.phoneNumber, c.address,
                       ts.timeSlotId AS tsId, ts.startTime, ts.endTime
                FROM [Order] o
                JOIN [User]   u  ON o.customerId  = u.userId
                JOIN Customer c  ON u.userId       = c.userId
                LEFT JOIN TimeSlot ts ON o.timeSlotId = ts.timeSlotId
                WHERE o.customerId = @customerId
                  AND o.status != 'InTheCart'";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@customerId", customerId);

                try
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        var orders = new List<Order>();
                        while (await reader.ReadAsync())
                            orders.Add(ReadOrder(reader));
                        // Sort by date descending so the most recent order appears first.
                        return orders.OrderByDescending(o => o.OrderDate).ThenBy(o => o.OrderId).ToList();
                    }
                }
                catch (SqlException ex)
                {
                    throw new DatabaseException("Failed to retrieve orders for customer.", ex);
                }
            }
        }
    }

    // Sets the time slot and derives the pickup date from it in a single UPDATE,
    // avoiding a separate SELECT to fetch the slot's startTime.
    public async Task SetTimeSlotAsync(int orderId, int timeSlotId)
    {
        using (SqlConnection conn = _db.GetConnexion())
        {
            await conn.OpenAsync();

            string query = @"
                UPDATE [Order]
                SET timeSlotId = @timeSlotId,
                    pickupDate = (SELECT startTime FROM TimeSlot WHERE timeSlotId = @timeSlotId)
                WHERE orderId = @orderId";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@orderId",    orderId);
                cmd.Parameters.AddWithValue("@timeSlotId", timeSlotId);
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }

    // Shared mapping method used by all query methods to avoid duplicating the reader logic.
    private static Order ReadOrder(SqlDataReader reader)
    {
        // pickupDate can be NULL for orders still in the cart (no slot chosen yet).
        DateTime pickupDate = reader["pickupDate"] == DBNull.Value
            ? DateTime.MinValue
            : (DateTime)reader["pickupDate"];

        var customer = new Customer(
            (int)reader["userId"],
            (string)reader["firstName"],
            (string)reader["lastName"],
            (string)reader["email"],
            (string)reader["password"],
            (int)reader["loyaltyPoints"],
            Convert.ToInt32(reader["phoneNumber"]),
            reader["address"] == DBNull.Value ? null : (string)reader["address"]
        );

        // tsId is NULL when no time slot has been assigned (LEFT JOIN returns NULL).
        TimeSlot? timeSlot = null;
        if (reader["tsId"] != DBNull.Value)
        {
            timeSlot = new TimeSlot(
                (int)reader["tsId"],
                (DateTime)reader["startTime"],
                (DateTime)reader["endTime"]
            );
        }

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
            null // store is not stored in the Order table — set separately via SetStore() when needed
        );
    }
}

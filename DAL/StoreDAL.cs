using Microsoft.Data.SqlClient;
using ClickAndGoApp.Models;

namespace ClickAndGoApp.DAL;

public class StoreDAL : IStoreDAL
{
    private readonly DBConnection _db;

    public StoreDAL(DBConnection db)
    {
        _db = db;
    }

    public async Task<Store> GetStoreAsync(int storeId)
    {
        using SqlConnection conn = _db.GetConnexion();
        await conn.OpenAsync();

        string query = @"
            SELECT storeId, name, address
            FROM Store
            WHERE storeId = @storeId";

        using SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@storeId", storeId);

        using SqlDataReader reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Store(
                (int)reader["storeId"],
                (string)reader["name"],
                (string)reader["address"]
            );
        }
        return null;
    }

    public async Task<List<Store>> GetAllStoresAsync()
    {
        using SqlConnection conn = _db.GetConnexion();
        await conn.OpenAsync();

        string query = "SELECT storeId, name, address FROM Store";
        using SqlCommand cmd = new SqlCommand(query, conn);
        using SqlDataReader reader = await cmd.ExecuteReaderAsync();

        var stores = new List<Store>();
        while (await reader.ReadAsync())
            stores.Add(new Store((int)reader["storeId"], (string)reader["name"], (string)reader["address"]));
        return stores;
    }

    public async Task<List<TimeSlot>> GetAvailableTimeSlotsAsync(int storeId)
    {
        using SqlConnection conn = _db.GetConnexion();
        await conn.OpenAsync();

        string query = @"
            SELECT ts.timeSlotId, ts.startTime, ts.endTime
            FROM TimeSlot ts
            WHERE ts.storeId = @storeId
              AND ts.startTime > GETDATE()
              AND (SELECT COUNT(*) FROM [Order] o WHERE o.timeSlotId = ts.timeSlotId) < 10
            ORDER BY ts.startTime";

        using SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@storeId", storeId);
        using SqlDataReader reader = await cmd.ExecuteReaderAsync();

        var slots = new List<TimeSlot>();
        while (await reader.ReadAsync())
            slots.Add(new TimeSlot((int)reader["timeSlotId"], (DateTime)reader["startTime"], (DateTime)reader["endTime"]));
        return slots;
    }
}

using Microsoft.Data.SqlClient;
using ClickAndGoApp.Models;

namespace ClickAndGoApp.DAL;

public class TimeSlotDAL : ITimeSlotDAL
{
    private readonly DBConnection db;

    public TimeSlotDAL(DBConnection db)
    {
        this.db = db;
    }

    public async Task<List<TimeSlot>> GetAvailableTimeSlotsAsync(int storeId)
    {
        using SqlConnection conn = db.GetConnexion();
        await conn.OpenAsync();

        const string query = @"
            SELECT ts.timeSlotId, ts.startTime, ts.endTime
            FROM TimeSlot ts
            WHERE ts.storeId = @storeId
              AND ts.startTime > GETDATE()
              AND (
                  SELECT COUNT(*) FROM [Order] o
                  WHERE o.timeSlotId = ts.timeSlotId
                    AND o.status != 'InTheCart'
              ) < 10
            ORDER BY ts.startTime";

        using SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@storeId", storeId);
        using SqlDataReader reader = await cmd.ExecuteReaderAsync();

        var slots = new List<TimeSlot>();
        while (await reader.ReadAsync())
            slots.Add(new TimeSlot((int)reader["timeSlotId"], (DateTime)reader["startTime"], (DateTime)reader["endTime"]));
        return slots;
    }

    public async Task<int?> GetStoreIdAsync(int timeSlotId)
    {
        using SqlConnection conn = db.GetConnexion();
        await conn.OpenAsync();

        const string query = "SELECT storeId FROM TimeSlot WHERE timeSlotId = @timeSlotId";
        using SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@timeSlotId", timeSlotId);

        object? result = await cmd.ExecuteScalarAsync();
        return result is null ? null : (int)result;
    }
}

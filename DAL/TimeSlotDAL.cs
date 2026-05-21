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

    // Returns only time slots that are still in the future AND have fewer than 10 confirmed orders.
    // The capacity check is done with a correlated subquery rather than a JOIN + GROUP BY
    // to keep the query readable and avoid inflating the result set before filtering.
    public async Task<List<TimeSlot>> GetAvailableTimeSlotsAsync(int storeId)
    {
        using (SqlConnection conn = db.GetConnexion())
        {
            await conn.OpenAsync();

            const string query = @"
                SELECT ts.timeSlotId, ts.startTime, ts.endTime
                FROM TimeSlot ts
                WHERE ts.storeId = @storeId
                  AND ts.startTime > GETDATE()
                  AND (
                      SELECT COUNT(*) FROM [Order] o
                      WHERE o.timeSlotId = ts.timeSlotId
                        AND o.status != 'InTheCart'  -- only count confirmed orders, not abandoned carts
                  ) < 10";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@storeId", storeId);

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    var slots = new List<TimeSlot>();
                    while (await reader.ReadAsync())
                        slots.Add(new TimeSlot((int)reader["timeSlotId"], (DateTime)reader["startTime"], (DateTime)reader["endTime"]));
                    // Sort in C# rather than SQL to keep ordering logic consistent with other DALs.
                    return slots.OrderBy(ts => ts.StartTime).ToList();
                }
            }
        }
    }

    // Used after login to restore the selected store ID from an existing cart's time slot.
    public async Task<int?> GetStoreIdAsync(int timeSlotId)
    {
        using (SqlConnection conn = db.GetConnexion())
        {
            await conn.OpenAsync();

            const string query = "SELECT storeId FROM TimeSlot WHERE timeSlotId = @timeSlotId";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@timeSlotId", timeSlotId);
                object? result = await cmd.ExecuteScalarAsync();
                return result is null ? null : (int)result;
            }
        }
    }
}

using ClickAndGoApp.DAL.interfaces;
using ClickAndGoApp.Models;
using Microsoft.Data.SqlClient;

namespace ClickAndGoApp.DAL;

public class StoreDAL : IStoreDAL
{
    private readonly DBConnection db;

    public StoreDAL(DBConnection db)
    {
        this.db = db;
    }
    
    public Store GetStore(int storeId)
    {
        using (SqlConnection conn = db.GetConnexion())
        {
            conn.Open();

            string query = @"
                SELECT storeId, name, address
                FROM Store
                WHERE storeId = @storeId";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@storeId", storeId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Store(
                            (int)reader["storeId"],
                            (string)reader["name"],
                            (string)reader["address"]
                        );
                    }
                }
            }
        }
        return null;
    }

    public List<Store> GetAllStores() => null;
    public List<TimeSlot> GetAvailableTimeSlots(int storeId) => null;
    
    async Task<Store> IStoreDAL.GetStore(int storeId)
    {
        using (SqlConnection conn = db.GetConnexion())
        {
            await conn.OpenAsync();

            string query = @"
                SELECT storeId, name, address
                FROM Store
                WHERE storeId = @storeId";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@storeId", storeId);

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Store(
                            (int)reader["storeId"],
                            (string)reader["name"],
                            (string)reader["address"]
                        );
                    }
                }
            }
        }
        return null;
    }

    Task<List<Store>> IStoreDAL.GetAllStores() => Task.FromResult<List<Store>>(null);
    Task<List<TimeSlot>> IStoreDAL.GetAvailableTimeSlots(int storeId) => Task.FromResult<List<TimeSlot>>(null);
}

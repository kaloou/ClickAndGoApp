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
        using (SqlConnection conn = _db.GetConnexion())
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
                    return null;
                }
            }
        }
    }

    public async Task<List<Store>> GetAllStoresAsync()
    {
        using (SqlConnection conn = _db.GetConnexion())
        {
            await conn.OpenAsync();

            string query = "SELECT storeId, name, address FROM Store";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    var stores = new List<Store>();
                    while (await reader.ReadAsync())
                        stores.Add(new Store((int)reader["storeId"], (string)reader["name"], (string)reader["address"]));
                    return stores;
                }
            }
        }
    }
}

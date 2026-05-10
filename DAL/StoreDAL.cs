using ClickAndGoApp.Models;
using Microsoft.Data.SqlClient;
namespace ClickAndGoApp.DAL;

public class StoreDAL
{
    private readonly DBConnection _db;

    public StoreDAL(DBConnection db)
    {
        _db = db;
    }

    public Store GetStore(int storeId)
    {
        using SqlConnection conn = _db.GetConnexion();
        conn.Open();
        
        string query = @"
                SELECT storeId, name, address
                FROM Store
                WHERE storeId = @storeId";

        using SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@storeId", storeId);

        using SqlDataReader reader = cmd.ExecuteReader();

        if (reader.Read())
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
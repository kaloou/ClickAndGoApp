namespace ClickAndGoApp.DAL;
using Microsoft.Data.SqlClient;
using ClickAndGoApp.Models;

public class CategoryDAL
{
    private readonly DBConnection _db;

    public CategoryDAL(DBConnection db) => _db = db;

    public List<Category> GetAll()
    {
        using SqlConnection conn = _db.GetConnexion();
        conn.Open();

        string query = "SELECT categoryId, name FROM Category ORDER BY name";
        using SqlCommand cmd = new SqlCommand(query, conn);
        using SqlDataReader reader = cmd.ExecuteReader();

        List<Category> categories = new();
        while (reader.Read())
            categories.Add(new Category((int)reader["categoryId"], (string)reader["name"]));

        return categories;
    }
}

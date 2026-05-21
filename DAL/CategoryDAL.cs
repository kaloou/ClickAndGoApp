using Microsoft.Data.SqlClient;
using ClickAndGoApp.Models;
using ClickAndGoApp.Exceptions;

namespace ClickAndGoApp.DAL;

public class CategoryDAL : ICategoryDAL
{
    private readonly DBConnection db;

    public CategoryDAL(DBConnection db)
    {
        this.db = db;
    }

    public async Task<List<Category>> GetAllAsync()
    {
        List<Category> categories = new List<Category>();

        try
        {
            using (SqlConnection conn = db.GetConnexion())
            {
                await conn.OpenAsync();

                string query = "SELECT categoryId, name FROM Category";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                            categories.Add(new Category((int)reader["categoryId"], (string)reader["name"]));
                    }
                }
            }
        }
        catch (SqlException e)
        {
            throw new DatabaseException("Failed to retrieve categories.", e);
        }

        return categories.OrderBy(c => c.Name).ToList();
    }
}

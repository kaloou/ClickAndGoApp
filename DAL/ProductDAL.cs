using Microsoft.Data.SqlClient;
using ClickAndGoApp.Models;

namespace ClickAndGoApp.DAL;

public class ProductDAL : IProductDAL
{
    private readonly DBConnection db;

    public ProductDAL(DBConnection db)
    {
        this.db = db;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        List<Product> products = new List<Product>();

        using (SqlConnection conn = db.GetConnexion())
        {
            await conn.OpenAsync();

            string query = @"
                SELECT p.productId, p.name, p.price, p.description, p.imagePath,
                       c.categoryId, c.name AS categoryName
                FROM Product p
                JOIN Category c ON p.categoryId = c.categoryId";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                        products.Add(ReadProduct(reader));
                }
            }
        }

        return products.OrderBy(p => p.Name).ToList();
    }

    public async Task<List<Product>> GetByCategoryAsync(int categoryId)
    {
        List<Product> products = new List<Product>();

        using (SqlConnection conn = db.GetConnexion())
        {
            await conn.OpenAsync();

            string query = @"
                SELECT p.productId, p.name, p.price, p.description, p.imagePath,
                       c.categoryId, c.name AS categoryName
                FROM Product p
                JOIN Category c ON p.categoryId = c.categoryId
                WHERE p.categoryId = @categoryId";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@categoryId", categoryId);

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                        products.Add(ReadProduct(reader));
                }
            }
        }

        return products.OrderBy(p => p.Name).ToList();
    }

    public async Task<Product> GetByIdAsync(int productId)
    {
        using (SqlConnection conn = db.GetConnexion())
        {
            await conn.OpenAsync();

            string query = @"
                SELECT p.productId, p.name, p.price, p.description, p.imagePath,
                       c.categoryId, c.name AS categoryName
                FROM Product p
                JOIN Category c ON p.categoryId = c.categoryId
                WHERE p.productId = @productId";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@productId", productId);

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync()) return null;
                    return ReadProduct(reader);
                }
            }
        }
    }

    private static Product ReadProduct(SqlDataReader reader)
    {
        var category = new Category(
            (int)reader["categoryId"],
            (string)reader["categoryName"]
        );

        return new Product(
            (int)reader["productId"],
            (string)reader["name"],
            (float)(decimal)reader["price"],
            category,
            reader["description"] == DBNull.Value ? null : (string)reader["description"],
            reader["imagePath"]   == DBNull.Value ? null : (string)reader["imagePath"]
        );
    }
}

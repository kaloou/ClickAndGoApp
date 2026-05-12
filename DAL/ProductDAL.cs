namespace ClickAndGoApp.DAL;
using Microsoft.Data.SqlClient;
using ClickAndGoApp.Models;

public class ProductDAL
{
    private readonly DBConnection _db;

    public ProductDAL(DBConnection db) => _db = db;

    public List<Product> GetFiltered(string search, int? categoryId)
    {
        using SqlConnection conn = _db.GetConnexion();
        conn.Open();

        string query = "SELECT productId, name, price, categoryId, description, imagePath FROM Product WHERE 1=1";

        if (!string.IsNullOrWhiteSpace(search))
            query += " AND name LIKE @search";
        if (categoryId.HasValue)
            query += " AND categoryId = @categoryId";

        query += " ORDER BY name";

        using SqlCommand cmd = new SqlCommand(query, conn);

        if (!string.IsNullOrWhiteSpace(search))
            cmd.Parameters.AddWithValue("@search", $"%{search}%");
        if (categoryId.HasValue)
            cmd.Parameters.AddWithValue("@categoryId", categoryId.Value);

        using SqlDataReader reader = cmd.ExecuteReader();

        List<Product> products = new();
        while (reader.Read())
            products.Add(new Product(
                (int)reader["productId"],
                (string)reader["name"],
                (float)(decimal)reader["price"],
                (int)reader["categoryId"],
                reader["description"] == DBNull.Value ? null : (string)reader["description"],
                reader["imagePath"] == DBNull.Value ? null : (string)reader["imagePath"]
            ));

        return products;
    }

    public ProductDetailViewModel GetById(int productId)
    {
        using SqlConnection conn = _db.GetConnexion();
        conn.Open();

        string query = @"
            SELECT p.productId, p.name, p.price, p.categoryId, p.description, p.imagePath, c.name AS categoryName
            FROM Product p
            JOIN Category c ON p.categoryId = c.categoryId
            WHERE p.productId = @productId";

        using SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@productId", productId);

        using SqlDataReader reader = cmd.ExecuteReader();

        if (!reader.Read()) return null;

        return new ProductDetailViewModel
        {
            Product = new Product(
                (int)reader["productId"],
                (string)reader["name"],
                (float)(decimal)reader["price"],
                (int)reader["categoryId"],
                reader["description"] == DBNull.Value ? null : (string)reader["description"],
                reader["imagePath"] == DBNull.Value ? null : (string)reader["imagePath"]
            ),
            CategoryName = (string)reader["categoryName"]
        };
    }
}

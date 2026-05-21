using ClickAndGoApp.Models;
using Microsoft.Data.SqlClient;

namespace ClickAndGoApp.DAL;

public class RecipeIngredientDAL : IRecipeIngredientDAL
{
    private readonly DBConnection db;

    public RecipeIngredientDAL(DBConnection db)
    {
        this.db = db;
    }

    public async Task<List<RecipeIngredient>> GetByRecipeAsync(int recipeId)
    {
        using (SqlConnection conn = db.GetConnexion())
        {
            await conn.OpenAsync();

            const string query = @"
                SELECT ri.recipeId, ri.productId, ri.quantity,
                       p.name, p.price, p.description, p.imagePath,
                       c.categoryId, c.name AS categoryName
                FROM RecipesIngredients ri
                JOIN Product  p ON ri.productId = p.productId
                JOIN Category c ON p.categoryId = c.categoryId
                WHERE ri.recipeId = @recipeId";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@recipeId", recipeId);

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    var ingredients = new List<RecipeIngredient>();
                    while (await reader.ReadAsync())
                    {
                        var category = new Category(
                            (int)reader["categoryId"],
                            (string)reader["categoryName"]
                        );
                        var product = new Product(
                            (int)reader["productId"],
                            (string)reader["name"],
                            (float)(decimal)reader["price"],
                            category,
                            reader["description"] == DBNull.Value ? null : (string)reader["description"],
                            reader["imagePath"]   == DBNull.Value ? null : (string)reader["imagePath"]
                        );
                        ingredients.Add(new RecipeIngredient(
                            (int)reader["recipeId"],
                            (int)reader["productId"],
                            (int)reader["quantity"],
                            product
                        ));
                    }
                    return ingredients;
                }
            }
        }
    }
}

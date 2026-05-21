using ClickAndGoApp.Models;
using ClickAndGoApp.Exceptions;
using Microsoft.Data.SqlClient;

namespace ClickAndGoApp.DAL;

public class RecipeDAL : IRecipeDAL
{
    private readonly DBConnection db;

    public RecipeDAL(DBConnection db)
    {
        this.db = db;
    }

    // Fetches only the recipe header (no ingredients) — enough for the listing page.
    public async Task<List<Recipe>> GetAllAsync()
    {
        using (SqlConnection conn = db.GetConnexion())
        {
            await conn.OpenAsync();

            const string query = "SELECT recipeId, name, description, imagePath FROM Recipes";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    var recipes = new List<Recipe>();
                    while (await reader.ReadAsync())
                    {
                        recipes.Add(new Recipe(
                            (int)reader["recipeId"],
                            (string)reader["name"],
                            (string)reader["description"]
                        ) { ImagePath = reader["imagePath"] == DBNull.Value ? null : (string)reader["imagePath"] });
                    }
                    return recipes;
                }
            }
        }
    }

    // Fetches a recipe with all its ingredients in a single query using LEFT JOINs.
    // LEFT JOIN is used instead of INNER JOIN so that a recipe with no ingredients is still returned.
    // The result has one row per ingredient, so we build the Recipe object on the first row
    // and append ingredients on every subsequent row.
    public async Task<Recipe> GetByIdAsync(int recipeId)
    {
        using (SqlConnection conn = db.GetConnexion())
        {
            await conn.OpenAsync();

            const string query = @"
                SELECT r.recipeId, r.name, r.description, r.imagePath AS recipeImagePath,
                       ri.productId, ri.quantity,
                       p.name AS productName, p.price, p.description AS productDescription, p.imagePath AS productImagePath,
                       c.categoryId, c.name AS categoryName
                FROM Recipes r
                LEFT JOIN RecipesIngredients ri ON r.recipeId = ri.recipeId
                LEFT JOIN Product p           ON ri.productId = p.productId
                LEFT JOIN Category c          ON p.categoryId = c.categoryId
                WHERE r.recipeId = @recipeId";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@recipeId", recipeId);

                try
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        Recipe recipe = null;
                        while (await reader.ReadAsync())
                        {
                            // Build the Recipe object only once — it's repeated on every row due to the JOIN.
                            if (recipe == null)
                            {
                                recipe = new Recipe(
                                    (int)reader["recipeId"],
                                    (string)reader["name"],
                                    (string)reader["description"]
                                ) { ImagePath = reader["recipeImagePath"] == DBNull.Value ? null : (string)reader["recipeImagePath"] };
                            }

                            // productId is NULL when the recipe has no ingredients (LEFT JOIN produces a NULL row).
                            if (reader["productId"] != DBNull.Value)
                            {
                                var category = new Category(
                                    (int)reader["categoryId"],
                                    (string)reader["categoryName"]
                                );
                                var product = new Product(
                                    (int)reader["productId"],
                                    (string)reader["productName"],
                                    (float)(decimal)reader["price"],
                                    category,
                                    reader["productDescription"] == DBNull.Value ? null : (string)reader["productDescription"],
                                    reader["productImagePath"]   == DBNull.Value ? null : (string)reader["productImagePath"]
                                );
                                // AddLast keeps ingredients in the order they come from the DB.
                                recipe.Ingredients.AddLast(new RecipeIngredient(
                                    (int)reader["recipeId"],
                                    (int)reader["productId"],
                                    (int)reader["quantity"],
                                    product
                                ));
                            }
                        }

                        if (recipe == null)
                            throw new EntityNotFoundException("Recipe", recipeId);

                        return recipe;
                    }
                }
                catch (SqlException ex)
                {
                    throw new DatabaseException("Failed to retrieve recipe.", ex);
                }
            }
        }
    }
}

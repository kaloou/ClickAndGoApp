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
                            if (reader["productId"] == DBNull.Value)
                                continue;

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
                            var ingredient = new RecipeIngredient(
                                product,
                                (int)reader["quantity"]
                            );

                            if (recipe == null)
                            {
                                // 1er ingrédient → constructeur composition 1..*
                                recipe = new Recipe(
                                    (int)reader["recipeId"],
                                    (string)reader["name"],
                                    (string)reader["description"],
                                    ingredient
                                ) { ImagePath = reader["recipeImagePath"] == DBNull.Value ? null : (string)reader["recipeImagePath"] };
                            }
                            else
                            {
                                recipe.AddIngredient(ingredient);
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

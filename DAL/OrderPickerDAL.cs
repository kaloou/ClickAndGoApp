using Microsoft.Data.SqlClient;
using ClickAndCollect.Models;
using ClickAndGoApp.DAL;
using ClickAndGoApp.Models;

namespace ClickAndGoApp.DAL
{
    public class OrderPickerDAL
    {
        private readonly DBConnection _db;

        public OrderPickerDAL(DBConnection db)
        {
            _db = db;
        }

        public OrderPicker GetById(int pickerId)
        {
            using SqlConnection conn = _db.GetConnexion();
            conn.Open();

            string query = @"
                SELECT u.userId, u.firstName, u.lastName, 
                       u.email, u.password, e.storeId
                FROM [User] u
                JOIN Employee e    ON u.userId = e.userId
                JOIN OrderPicker op ON e.userId = op.userId
                WHERE op.userId = @pickerId";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@pickerId", pickerId);

            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new OrderPicker(
                    (int)reader["userId"],
                    (string)reader["firstName"],
                    (string)reader["lastName"],
                    (string)reader["email"],
                    (string)reader["password"],
                    (int)reader["storeId"]
                );
            }
            return null;
        }
    }
}
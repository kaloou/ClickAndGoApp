using Microsoft.Data.SqlClient;
using ClickAndGoApp.Models;
using ClickAndGoApp.DAL.interfaces;

namespace ClickAndGoApp.DAL
{
    public class OrderPickerDAL : IOrderPickerDAL
    {
        private readonly DBConnection _db;

        public OrderPickerDAL(DBConnection db)
        {
            _db = db;
        }

        private static readonly string BaseQuery = @"
            SELECT u.userId, u.firstName, u.lastName, u.email, u.password,
                   s.storeId, s.name AS storeName, s.address AS storeAddress
            FROM [User] u
            JOIN Employee    e  ON u.userId = e.userId
            JOIN OrderPicker op ON e.userId = op.userId
            JOIN Store       s  ON e.storeId = s.storeId";

        private static OrderPicker ReadOrderPicker(SqlDataReader reader)
        {
            var store = new Store(
                (int)reader["storeId"],
                (string)reader["storeName"],
                (string)reader["storeAddress"]
            );

            return new OrderPicker(
                (int)reader["userId"],
                (string)reader["firstName"],
                (string)reader["lastName"],
                (string)reader["email"],
                (string)reader["password"],
                store
            );
        }
        
        // mieux d'utiliser IOrderPickerDal
        public OrderPicker GetById(int pickerId)
        {
            using SqlConnection conn = _db.GetConnexion();
            conn.Open();

            string query = BaseQuery + " WHERE op.userId = @pickerId";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@pickerId", pickerId);

            using SqlDataReader reader = cmd.ExecuteReader();
            return reader.Read() ? ReadOrderPicker(reader) : null;
        }
        
        async Task<OrderPicker> IOrderPickerDAL.GetById(int pickerId)
        {
            using SqlConnection conn = _db.GetConnexion();
            await conn.OpenAsync();

            string query = BaseQuery + " WHERE op.userId = @pickerId";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@pickerId", pickerId);

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? ReadOrderPicker(reader) : null;
        }
    }
}

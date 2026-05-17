using Microsoft.Data.SqlClient;

namespace ClickAndGoApp.DAL;
public class DBConnection
{
    private readonly string connectionString;

    public DBConnection(IConfiguration configuration)
        => connectionString = configuration.GetConnectionString("DefaultConnection");

    public SqlConnection GetConnexion()
        => new SqlConnection(connectionString);
}
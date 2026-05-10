namespace ClickAndGoApp.DAL;
using Microsoft.Data.SqlClient;
public class DBConnection
{
    private readonly string connectionString;

    public DBConnection(IConfiguration configuration)
        => connectionString = configuration.GetConnectionString("DefaultConnection");

    public SqlConnection GetConnexion()
        => new SqlConnection(connectionString);
}
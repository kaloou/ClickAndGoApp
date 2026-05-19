using Microsoft.Data.SqlClient;

namespace ClickAndGoApp.DAL;

public class DBConnection
{
    private readonly string connectionString;
    private static DBConnection instance = null;

    private DBConnection(IConfiguration configuration)
        => connectionString = configuration.GetConnectionString("DefaultConnection");
    //Singleton
    //Dependency injection(DI)
    public static DBConnection GetInstance(IConfiguration configuration)
    {
        if (instance == null)
            instance = new DBConnection(configuration);
        return instance;
    }
    
    public SqlConnection GetConnexion()
        => new SqlConnection(connectionString);
}

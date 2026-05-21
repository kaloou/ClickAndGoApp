using Microsoft.Data.SqlClient;

namespace ClickAndGoApp.DAL;

// Singleton pattern — only one DBConnection instance exists for the lifetime of the application.
// This avoids re-reading the connection string from configuration on every request
// and ensures all DALs share the same connection factory.
// Note: each method call still opens a fresh SqlConnection (from the pool), so this is thread-safe.
public class DBConnection
{
    private readonly string connectionString;
    private static DBConnection instance = null;

    private DBConnection(IConfiguration configuration)
        => connectionString = configuration.GetConnectionString("DefaultConnection");

    // GetInstance is called once at startup via dependency injection (Program.cs AddSingleton).
    // Subsequent calls return the same instance without re-reading the config.
    public static DBConnection GetInstance(IConfiguration configuration)
    {
        if (instance == null)
            instance = new DBConnection(configuration);
        return instance;
    }

    // Returns a new SqlConnection each time — ADO.NET connection pooling handles the underlying reuse.
    // We don't keep a single open connection because that would cause issues under concurrent requests.
    public SqlConnection GetConnexion()
        => new SqlConnection(connectionString);
}

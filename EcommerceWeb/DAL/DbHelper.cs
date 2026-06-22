using Microsoft.Data.SqlClient;

namespace EcommerceWeb.DAL
{
    public class DbHelper
    {
        // Set once at startup from configuration (see Program.cs).
        // Never hard-code connection strings or credentials in source.
        public static string ConnectionString { get; set; } = string.Empty;

        public SqlConnection GetConnection() => new SqlConnection(ConnectionString);
    }
}

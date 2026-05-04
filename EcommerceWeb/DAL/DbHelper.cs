using Microsoft.Data.SqlClient;

namespace EcommerceWeb.DAL
{
    public class DbHelper
    {
        private readonly string _connectionString =
            "Server=test.cgdk2ewisgbk.us-east-1.rds.amazonaws.com,1433;" +
            "Database=web_database;User Id=admin;Password=zainchuttikr;" +
            "TrustServerCertificate=True;";

        public SqlConnection GetConnection() => new SqlConnection(_connectionString);
    }
}
using Microsoft.Data.SqlClient;
using System.Data;

namespace MyApp.Services
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public DbConnectionFactory(string connectionString)
        {
            // Adaugă timeout mai mare dacă nu există deja
            var builder = new SqlConnectionStringBuilder(connectionString);
            if (builder.ConnectTimeout < 60)
            {
                builder.ConnectTimeout = 60; // 60 secunde pentru conexiune
            }
            builder.CommandTimeout = 120; // 120 secunde pentru comenzi
            _connectionString = builder.ConnectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
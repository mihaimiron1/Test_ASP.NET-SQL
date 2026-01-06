using System.Data;

namespace MyApp.Services
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
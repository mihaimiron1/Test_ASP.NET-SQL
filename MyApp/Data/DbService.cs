using Dapper;
using Microsoft.Data.SqlClient;
using MyApp.Models;
using System.Data;

namespace MyApp.Data
{
    public class DbService
    {
        private readonly IConfiguration _config;

        public DbService(IConfiguration config)
        {
            _config = config;
        }

        private IDbConnection Connection =>
            new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        // EX: procedură care returnează tabel
        public async Task<List<RegionStatistic>> GetVoterStatistics()
        {
            using var db = Connection;
            var result = await db.QueryAsync<RegionStatistic>(
                "GetVoterStatistics",
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }
    }
}

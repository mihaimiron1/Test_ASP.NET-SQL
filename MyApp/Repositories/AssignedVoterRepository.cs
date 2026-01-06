using Dapper;
using MyApp.Models;
using MyApp.Services;
using System.Data;

namespace MyApp.Repositories
{
    public class AssignedVoterRepository : IAssignedVoterRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AssignedVoterRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<AssignedVoterStatistics>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"SELECT 
                            AssignedVoterId,
                            AssignedVoterStatus,
                            Gender,
                            AgeCategoryId,
                            PollingStationId,
                            RegionId,
                            ParentRegionId,
                            CreationDate
                        FROM AssignedVoterStatistics";

            return await connection.QueryAsync<AssignedVoterStatistics>(sql);
        }

        

        public async Task<IEnumerable<AssignedVoterStatistics>> GetByRegionAsync(int regionId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"SELECT 
                            AssignedVoterId,
                            AssignedVoterStatus,
                            Gender,
                            AgeCategoryId,
                            PollingStationId,
                            RegionId,
                            ParentRegionId,
                            CreationDate
                        FROM AssignedVoterStatistics
                        WHERE RegionId = @RegionId";

            return await connection.QueryAsync<AssignedVoterStatistics>(sql, new { RegionId = regionId });
        }

        public async Task<IEnumerable<AssignedVoterStatistics>> GetByParentRegionAsync(int regionId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"SELECT 
                            AssignedVoterId,
                            AssignedVoterStatus,
                            Gender,
                            AgeCategoryId,
                            PollingStationId,
                            RegionId,
                            ParentRegionId,
                            CreationDate
                        FROM AssignedVoterStatistics
                        WHERE ParentRegionId = @ParentRegionId";

            return await connection.QueryAsync<AssignedVoterStatistics>(sql, new { ParentRegionId = regionId });
        }
    }
}
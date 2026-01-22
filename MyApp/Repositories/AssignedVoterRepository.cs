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

        public async Task<IEnumerable<RegionStatistic>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"SELECT TOP (100) 
                            AssignedVoterId,
                            AssignedVoterStatus,
                            Gender,
                            AgeCategoryId,
                            PollingStationId,
                            RegionId,
                            ParentRegionId,
                            CreationDate
                        FROM AssignedVoterStatistics";

            return await connection.QueryAsync<RegionStatistic>(sql);
        }

        

        public async Task<IEnumerable<RegionStatistic>> GetByRegionAsync(int regionId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"SELECT TOP (100) 
                            AssignedVoterId,v 
                            AssignedVoterStatus,
                            Gender,
                            AgeCategoryId,
                            PollingStationId,
                            RegionId,
                            ParentRegionId,
                            CreationDate
                        FROM AssignedVoterStatistics
                        WHERE RegionId = @RegionId";

            return await connection.QueryAsync<RegionStatistic>(sql, new { RegionId = regionId });
        }

        public async Task<IEnumerable<RegionStatistic>> GetByParentRegionAsync(int regionId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"SELECT TOP (100) 
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

            return await connection.QueryAsync<RegionStatistic>(sql, new { ParentRegionId = regionId });
        }
    }
}
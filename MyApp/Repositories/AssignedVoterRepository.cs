using Dapper;
using MyApp.Models;
using MyApp.Services;
using System.Data;            


namespace MyApp.Repositories
{
    public class AssignedVoterRepository : IAssignedVoterRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<AssignedVoterRepository> _logger;
        private const int CommandTimeout = 120;

        public AssignedVoterRepository(
            IDbConnectionFactory connectionFactory,
            ILogger<AssignedVoterRepository> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public async Task<IEnumerable<RegionStatistic>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Getting all assigned voters (TOP 100)");
                await using var connection = (System.Data.Common.DbConnection)_connectionFactory.CreateConnection();

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

                var result = await connection.QueryAsync<RegionStatistic>(sql, commandTimeout: CommandTimeout);
                _logger.LogInformation("GetAllAsync returned {Count} rows", result.Count());
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllAsync");
                throw;
            }
        }

        public async Task<IEnumerable<RegionStatistic>> GetByRegionAsync(int regionId)
        {
            try
            {
                _logger.LogInformation("Getting assigned voters for RegionId={RegionId}", regionId);
                await using var connection = (System.Data.Common.DbConnection)_connectionFactory.CreateConnection();

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
                            WHERE RegionId = @RegionId";

                var result = await connection.QueryAsync<RegionStatistic>(sql, new { RegionId = regionId }, commandTimeout: CommandTimeout);
                _logger.LogInformation("GetByRegionAsync for RegionId={RegionId} returned {Count} rows", regionId, result.Count());
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetByRegionAsync for RegionId={RegionId}", regionId);
                throw;
            }
        }

        public async Task<IEnumerable<RegionStatistic>> GetByParentRegionAsync(int regionId)
        {
            try
            {
                _logger.LogInformation("Getting assigned voters for ParentRegionId={ParentRegionId}", regionId);
                await using var connection = (System.Data.Common.DbConnection)_connectionFactory.CreateConnection();

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

                var result = await connection.QueryAsync<RegionStatistic>(sql, new { ParentRegionId = regionId }, commandTimeout: CommandTimeout);
                _logger.LogInformation("GetByParentRegionAsync for ParentRegionId={ParentRegionId} returned {Count} rows", regionId, result.Count());
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetByParentRegionAsync for ParentRegionId={ParentRegionId}", regionId);
                throw;
            }
        }
    }
}
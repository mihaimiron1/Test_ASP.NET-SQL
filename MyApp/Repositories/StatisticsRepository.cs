using Dapper;
using MyApp.Models.Statistics;
using MyApp.Services;
using System.Data;

namespace MyApp.Repositories
{
    public class StatisticsRepository : IStatisticsRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<StatisticsRepository> _logger;

        public StatisticsRepository(
            IDbConnectionFactory connectionFactory,
            ILogger<StatisticsRepository> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        // ============================================================
        // 1. Heat Map - Toate regiunile cu prezență la vot
        // ============================================================
        public async Task<IEnumerable<RegionVotingStatistic>> GetAllRegionsStatisticsAsync()
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                return await connection.QueryAsync<RegionVotingStatistic>(
                    "sp_GetRaionVotingStatsForHeatMap",
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling sp_GetRaionVotingStatsForHeatMap");
                throw;
            }
        }

        // ============================================================
        // 2. Statistici detaliate pentru RAION (RegionTypeId IN 2,3,4)
        // ============================================================
        public async Task<DetailedRegionStatistic> GetRegionDetailedStatisticsAsync(int regionId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                var result = new DetailedRegionStatistic
                {
                    RegionId = regionId,
                    LastUpdated = DateTime.Now
                };

                // Get region name and basic stats
                var regionInfoSql = @"
                    SELECT 
                        r.RegionId,
                        r.Name AS RegionName,
                        COUNT(*) AS TotalVoters,
                        SUM(CASE WHEN avs.AssignedVoterStatus >= 5000 THEN 1 ELSE 0 END) AS VotedCount
                    FROM AssignedVoterStatistics avs
                    INNER JOIN dbo.Region r ON avs.RegionId = r.RegionId
                    WHERE r.RegionId = @RegionId
                    GROUP BY r.RegionId, r.Name";

                var regionInfo = await connection.QueryFirstOrDefaultAsync<dynamic>(
                    regionInfoSql,
                    new { RegionId = regionId });

                if (regionInfo != null)
                {
                    result.RegionName = regionInfo.RegionName;
                    result.TotalVoters = regionInfo.TotalVoters;
                    result.VotedCount = regionInfo.VotedCount;
                }

                // Get gender statistics using stored procedure
                var genderStatsRaw = await connection.QueryAsync<dynamic>(
                    "sp_GetRaionGender",
                    new { RegionId = regionId },
                    commandType: CommandType.StoredProcedure);

                var genderStats = genderStatsRaw.Select(g => new GenderStatistic
                {
                    Gender = (int)g.Gender switch
                    {
                        1 => "Barbati",
                        2 => "Femei",
                        _ => "Necunoscut"
                    },
                    VoterCount = (int)g.VotedCount, // Use VotedCount from stored procedure
                    VotedCount = (int)g.VotedCount,
                    Percentage = 0 // Will calculate below
                }).ToList();

                // Calculate percentages based on VotedCount
                var totalVoted = genderStats.Sum(g => g.VoterCount);
                foreach (var stat in genderStats)
                {
                    stat.Percentage = totalVoted > 0
                        ? Math.Round((decimal)stat.VoterCount / totalVoted * 100, 2)
                        : 0;
                }
                result.GenderStats = genderStats;

                // Get age statistics using stored procedure
                var ageStats = await connection.QueryAsync<AgeStatistic>(
                    "sp_GetRaionAgeCategory",
                    new { RegionId = regionId },
                    commandType: CommandType.StoredProcedure);

                // Calculate percentages
                foreach (var stat in ageStats)
                {
                    stat.Percentage = totalVoted > 0
                        ? Math.Round((decimal)stat.VoterCount / totalVoted * 100, 2)
                        : 0;
                }
                result.AgeStats = ageStats.ToList();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting region detailed statistics for RegionId={RegionId}", regionId);
                throw;
            }
        }

        // ============================================================
        // 3. Statistici detaliate pentru LOCALITATE
        // ============================================================
        public async Task<DetailedRegionStatistic> GetLocalityDetailedStatisticsAsync(int regionId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                var result = new DetailedRegionStatistic
                {
                    RegionId = regionId,
                    LastUpdated = DateTime.Now
                };

                // Get locality name and basic stats
                var localityInfoSql = @"
                    SELECT 
                        r.RegionId,
                        r.Name AS RegionName,
                        COUNT(*) AS TotalVoters,
                        SUM(CASE WHEN avs.AssignedVoterStatus >= 5000 THEN 1 ELSE 0 END) AS VotedCount
                    FROM AssignedVoterStatistics avs
                    INNER JOIN dbo.Region r ON avs.RegionId = r.RegionId
                    WHERE r.RegionId = @RegionId
                    GROUP BY r.RegionId, r.Name";

                var localityInfo = await connection.QueryFirstOrDefaultAsync<dynamic>(
                    localityInfoSql,
                    new { RegionId = regionId });

                if (localityInfo != null)
                {
                    result.RegionName = localityInfo.RegionName;
                    result.TotalVoters = localityInfo.TotalVoters;
                    result.VotedCount = localityInfo.VotedCount;
                }

                // Get gender statistics using stored procedure
                var genderStatsRaw = await connection.QueryAsync<dynamic>(
                    "sp_GetLocalitateGender",
                    new { RegionId = regionId },
                    commandType: CommandType.StoredProcedure);

                var genderStats = genderStatsRaw.Select(g => new GenderStatistic
                {
                    Gender = (int)g.Gender switch
                    {
                        1 => "Barbati",
                        2 => "Femei",
                        _ => "Necunoscut"
                    },
                    VoterCount = (int)g.VoterCount,
                    VotedCount = (int)g.VoterCount, // For localitate, VoterCount = VotedCount (already filtered by status >= 5000)
                    Percentage = 0 // Will calculate below
                }).ToList();

                // Calculate percentages
                var totalVoted = genderStats.Sum(g => g.VoterCount);
                foreach (var stat in genderStats)
                {
                    stat.Percentage = totalVoted > 0
                        ? Math.Round((decimal)stat.VoterCount / totalVoted * 100, 2)
                        : 0;
                }
                result.GenderStats = genderStats;

                // Get age statistics using stored procedure
                var ageStats = await connection.QueryAsync<AgeStatistic>(
                    "sp_GetLocalitateAgeCategory",
                    new { RegionId = regionId },
                    commandType: CommandType.StoredProcedure);

                // Calculate percentages
                foreach (var stat in ageStats)
                {
                    stat.Percentage = totalVoted > 0
                        ? Math.Round((decimal)stat.VoterCount / totalVoted * 100, 2)
                        : 0;
                }
                result.AgeStats = ageStats.ToList();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting locality detailed statistics for RegionId={RegionId}", regionId);
                throw;
            }
        }

        // ============================================================
        // 4. Obține lista de localități dintr-un raion
        // ============================================================
        public async Task<IEnumerable<RegionBasicInfo>> GetLocalitiesByRegionAsync(int regionId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var results = await connection.QueryAsync<dynamic>(
                    "sp_GetLocalitiesByRaion",
                    new { RegionId = regionId },
                    commandType: CommandType.StoredProcedure);

                return results.Select(r => new RegionBasicInfo
                {
                    RegionId = r.RegionId,
                    Name = r.LocalitateName,
                    RegionTypeId = r.RegionTypeId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting localities for RegionId={RegionId}", regionId);
                throw;
            }
        }
    }
}
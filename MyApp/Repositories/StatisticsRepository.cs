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
        private const int CommandTimeout = 120;

        public StatisticsRepository(
            IDbConnectionFactory connectionFactory,
            ILogger<StatisticsRepository> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        // ============================================================
        // 1. Heat Map - Lista raioane
        // Apelează: sp_GetRaionVotingStatsForHeatMap (fără parametri)
        // Returnează: RegionId, Name, NameRu, TotalVoters, LastUpdated
        // ============================================================
        public async Task<IEnumerable<RegionVotingStatistic>> GetAllRegionsStatisticsAsync()
        {
            try
            {
                _logger.LogInformation("Calling sp_GetRaionVotingStatsForHeatMap...");
                await using var connection = (System.Data.Common.DbConnection)_connectionFactory.CreateConnection();
                
                var result = await connection.QueryAsync<RegionVotingStatistic>(
                    "sp_GetRaionVotingStatsForHeatMap",
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: CommandTimeout);
                
                _logger.LogInformation("sp_GetRaionVotingStatsForHeatMap returned {Count} rows", result.Count());
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling sp_GetRaionVotingStatsForHeatMap");
                throw;
            }
        }

        // ============================================================
        // 2. Statistici detaliate pentru RAION
        // Apelează: sp_GetRaionGender + sp_GetRaionAgeCategory
        // ============================================================
        public async Task<DetailedRegionStatistic> GetRegionDetailedStatisticsAsync(int regionId)
        {
            try
            {
                _logger.LogInformation("Getting detailed statistics for RegionId={RegionId}", regionId);
                await using var connection = (System.Data.Common.DbConnection)_connectionFactory.CreateConnection();

                var result = new DetailedRegionStatistic
                {
                    RegionId = regionId,
                    LastUpdated = DateTime.Now
                };

                // Apelează sp_GetRaionGender
                // Returnează: Gender (1/2), VoterCount, VotedCount
                _logger.LogInformation("Calling sp_GetRaionGender for RegionId={RegionId}", regionId);
                var genderStatsRaw = await connection.QueryAsync<dynamic>(
                    "sp_GetRaionGender",
                    new { RegionId = regionId },
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: CommandTimeout);

                var genderStats = genderStatsRaw.Select(g => new GenderStatistic
                {
                    Gender = (int)g.Gender switch
                    {
                        1 => "Barbati",
                        2 => "Femei",
                        _ => "Necunoscut"
                    },
                    VoterCount = (int)g.VotedCount,
                    VotedCount = (int)g.VotedCount,
                    Percentage = 0
                }).ToList();

                var totalVoted = genderStats.Sum(g => g.VoterCount);
                foreach (var stat in genderStats)
                {
                    stat.Percentage = totalVoted > 0
                        ? Math.Round((decimal)stat.VoterCount / totalVoted * 100, 2)
                        : 0;
                }
                result.GenderStats = genderStats;

                // Apelează sp_GetRaionAgeCategory
                // Returnează: AgeCategoryId, VoterCount
                _logger.LogInformation("Calling sp_GetRaionAgeCategory for RegionId={RegionId}", regionId);
                var ageStats = await connection.QueryAsync<AgeStatistic>(
                    "sp_GetRaionAgeCategory",
                    new { RegionId = regionId },
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: CommandTimeout);

                foreach (var stat in ageStats)
                {
                    stat.Percentage = totalVoted > 0
                        ? Math.Round((decimal)stat.VoterCount / totalVoted * 100, 2)
                        : 0;
                }
                result.AgeStats = ageStats.ToList();

                // Setează nume și totaluri
                result.RegionName = genderStats.Any() ? $"Region {regionId}" : "Unknown";
                result.TotalVoters = totalVoted;
                result.VotedCount = totalVoted;

                _logger.LogInformation("Completed statistics for RegionId={RegionId}: {TotalVoted} voted, {GenderCount} genders, {AgeCount} age groups",
                    regionId, totalVoted, genderStats.Count, ageStats.Count());
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting region statistics for RegionId={RegionId}", regionId);
                throw;
            }
        }

        // ============================================================
        // 3. Statistici detaliate pentru LOCALITATE
        // Apelează: sp_GetLocalitateGender + sp_GetLocalitateAgeCategory
        // ============================================================
        public async Task<DetailedRegionStatistic> GetLocalityDetailedStatisticsAsync(int regionId)
        {
            try
            {
                _logger.LogInformation("Getting locality statistics for RegionId={RegionId}", regionId);
                await using var connection = (System.Data.Common.DbConnection)_connectionFactory.CreateConnection();

                var result = new DetailedRegionStatistic
                {
                    RegionId = regionId,
                    LastUpdated = DateTime.Now
                };

                // Apelează sp_GetLocalitateGender
                // Returnează: Gender (1/2), VoterCount
                var genderStatsRaw = await connection.QueryAsync<dynamic>(
                    "sp_GetLocalitateGender",
                    new { RegionId = regionId },
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: CommandTimeout);

                var genderStats = genderStatsRaw.Select(g => new GenderStatistic
                {
                    Gender = (int)g.Gender switch
                    {
                        1 => "Barbati",
                        2 => "Femei",
                        _ => "Necunoscut"
                    },
                    VoterCount = (int)g.VoterCount,
                    VotedCount = (int)g.VoterCount,
                    Percentage = 0
                }).ToList();

                var totalVoted = genderStats.Sum(g => g.VoterCount);
                foreach (var stat in genderStats)
                {
                    stat.Percentage = totalVoted > 0
                        ? Math.Round((decimal)stat.VoterCount / totalVoted * 100, 2)
                        : 0;
                }
                result.GenderStats = genderStats;

                // Apelează sp_GetLocalitateAgeCategory
                // Returnează: AgeCategoryId, VoterCount
                var ageStats = await connection.QueryAsync<AgeStatistic>(
                    "sp_GetLocalitateAgeCategory",
                    new { RegionId = regionId },
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: CommandTimeout);

                foreach (var stat in ageStats)
                {
                    stat.Percentage = totalVoted > 0
                        ? Math.Round((decimal)stat.VoterCount / totalVoted * 100, 2)
                        : 0;
                }
                result.AgeStats = ageStats.ToList();

                result.RegionName = $"Locality {regionId}";
                result.TotalVoters = totalVoted;
                result.VotedCount = totalVoted;

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting locality statistics for RegionId={RegionId}", regionId);
                throw;
            }
        }

        // ============================================================
        // 4. Listă localități dintr-un raion
        // Apelează: sp_GetLocalitiesByRaion
        // Returnează: RegionId, LocalitateName, RegionTypeId
        // ============================================================
        public async Task<IEnumerable<RegionBasicInfo>> GetLocalitiesByRegionAsync(int regionId)
        {
            try
            {
                _logger.LogInformation("Calling sp_GetLocalitiesByRaion for RegionId={RegionId}", regionId);
                await using var connection = (System.Data.Common.DbConnection)_connectionFactory.CreateConnection();
                
                // Procedura așteaptă @RegionId ca BigInt
                var parameters = new DynamicParameters();
                parameters.Add("@RegionId", (long)regionId, DbType.Int64);
                
                var results = await connection.QueryAsync<dynamic>(
                    "sp_GetLocalitiesByRaion",
                    parameters,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: CommandTimeout);

                var resultsList = results.ToList();
                _logger.LogInformation("sp_GetLocalitiesByRaion returned {Count} raw rows", resultsList.Count);

                var localities = new List<RegionBasicInfo>();
                foreach (var r in resultsList)
                {
                    int locRegionId = (int)r.RegionId;
                    string locName = (string)r.LocalitateName;
                    int locTypeId = (int)r.RegionTypeId;
                    
                    localities.Add(new RegionBasicInfo
                    {
                        RegionId = locRegionId,
                        Name = locName,
                        RegionTypeId = locTypeId
                    });
                }

                _logger.LogInformation("GetLocalitiesByRegionAsync for RegionId={RegionId} returned {Count} localities", regionId, localities.Count);
                return localities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting localities for RegionId={RegionId}", regionId);
                throw;
            }
        }
    }   
}
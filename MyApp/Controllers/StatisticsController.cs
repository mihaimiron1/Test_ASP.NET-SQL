using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MyApp.Repositories;
using MyApp.Services;
using System.Text.Json;

namespace MyApp.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly IStatisticsRepository _statsRepository;
        private readonly IMemoryCache _cache;
        private readonly ILogger<StatisticsController> _logger;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public StatisticsController(
            IStatisticsRepository statsRepository,
            IMemoryCache cache,
            ILogger<StatisticsController> logger)
        {
            _statsRepository = statsRepository;
            _cache = cache;
            _logger = logger;
        }

        public IActionResult HeatMap()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetRegionStatisticsForHeatMap(int regionId)
        {
            try
            {
                // Încearcă din cache MAI ÎNTÂI
                var cacheKey = $"RegionDetails_{regionId}";
                var regionStats = _cache.Get<MyApp.Models.Statistics.DetailedRegionStatistic>(cacheKey);

                if (regionStats == null)
                {
                    _logger.LogInformation("Cache miss pentru RegionId={RegionId}, se încarcă din DB...", regionId);
                    regionStats = await _statsRepository.GetRegionDetailedStatisticsAsync(regionId);
                    
                    if (regionStats != null)
                    {
                        // Cache pentru 10 minute
                        _cache.Set(cacheKey, regionStats, TimeSpan.FromMinutes(10));
                    }
                }
                else
                {
                    _logger.LogInformation("Cache HIT pentru RegionId={RegionId}", regionId);
                }

                if (regionStats == null)
                {
                    return Json(new { success = false, message = "Nu s-au găsit statistici pentru această regiune." });
                }

                return Json(new
                {
                    success = true,
                    regionId = regionStats.RegionId,
                    regionName = regionStats.RegionName,
                    genderStats = regionStats.GenderStats.Select(g => new
                    {
                        gender = g.Gender,
                        voterCount = g.VoterCount,
                        votedCount = g.VotedCount,
                        percentage = g.Percentage,
                        color = g.Color
                    }),
                    ageStats = regionStats.AgeStats.Select(a => new
                    {
                        ageCategoryName = a.AgeCategoryName,
                        voterCount = a.VoterCount,
                        percentage = a.Percentage,
                        color = a.Color
                    }),
                    lastUpdated = regionStats.LastUpdated
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Eroare la GetRegionStatisticsForHeatMap pentru regionId={RegionId}", regionId);
                return Json(new { success = false, message = "Eroare la încărcarea datelor: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetLocalityStatistics(int regionId)
        {
            try
            {
                var cacheKey = $"LocalityDetails_{regionId}";
                var localityStats = _cache.Get<MyApp.Models.Statistics.DetailedRegionStatistic>(cacheKey);

                if (localityStats == null)
                {
                    localityStats = await _statsRepository.GetLocalityDetailedStatisticsAsync(regionId);
                    
                    if (localityStats != null)
                    {
                        _cache.Set(cacheKey, localityStats, TimeSpan.FromMinutes(10));
                    }
                }

                if (localityStats == null)
                {
                    return Json(new { success = false, message = "Nu s-au găsit statistici pentru această localitate." });
                }

                return Json(new
                {
                    success = true,
                    regionId = localityStats.RegionId,
                    regionName = localityStats.RegionName,
                    genderStats = localityStats.GenderStats.Select(g => new
                    {
                        gender = g.Gender,
                        voterCount = g.VoterCount,
                        votedCount = g.VotedCount,
                        percentage = g.Percentage,
                        color = g.Color
                    }),
                    ageStats = localityStats.AgeStats.Select(a => new
                    {
                        ageCategoryName = a.AgeCategoryName,
                        voterCount = a.VoterCount,
                        percentage = a.Percentage,
                        color = a.Color
                    }),
                    lastUpdated = localityStats.LastUpdated
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Eroare la GetLocalityStatistics pentru regionId={RegionId}", regionId);
                return Json(new { success = false, message = "Eroare la încărcarea datelor: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetLocalitiesByRegion(int regionId)
        {
            try
            {
                _logger.LogInformation("GetLocalitiesByRegion called for RegionId={RegionId}", regionId);
                
                var cacheKey = $"Localities_{regionId}";
                var localities = _cache.Get<List<MyApp.Models.Statistics.RegionBasicInfo>>(cacheKey);

                if (localities == null)
                {
                    _logger.LogInformation("Cache miss for Localities_{RegionId}, loading from DB...", regionId);
                    var results = await _statsRepository.GetLocalitiesByRegionAsync(regionId);
                    localities = results.ToList();
                    _logger.LogInformation("Loaded {Count} localities for RegionId={RegionId}", localities.Count, regionId);
                    _cache.Set(cacheKey, localities, TimeSpan.FromMinutes(10));
                }
                else
                {
                    _logger.LogInformation("Cache HIT for Localities_{RegionId}, {Count} localities", regionId, localities.Count);
                }

                return Json(new
                {
                    success = true,
                    localities = localities.Select(l => new
                    {
                        regionId = l.RegionId,
                        name = l.Name,
                        regionTypeId = l.RegionTypeId,
                        regionTypeName = l.RegionTypeName
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Eroare la GetLocalitiesByRegion pentru regionId={RegionId}", regionId);
                return Json(new { success = false, message = "Eroare la încărcarea localităților: " + ex.Message });
            }
        }

        // Endpoint pentru debug
        [HttpGet]
        public IActionResult CacheInfo()
        {
            var regionStats = _cache.Get<List<MyApp.Models.Statistics.RegionVotingStatistic>>("RegionStatistics");
            
            return Json(new
            {
                hasCachedData = regionStats != null,
                regionCount = regionStats?.Count ?? 0,
                regions = regionStats?.Select(r => new { r.RegionId, r.Name, r.TotalVoters })
            });
        }

        // Endpoint pentru încărcarea datelor hărții via AJAX
        [HttpGet]
        public async Task<IActionResult> GetMapData()
        {
            try
            {
                var regionStats = _cache.Get<List<MyApp.Models.Statistics.RegionVotingStatistic>>("RegionStatistics");

                if (regionStats == null)
                {
                    _logger.LogInformation("GetMapData: Cache miss, loading from DB...");
                    var stats = await _statsRepository.GetAllRegionsStatisticsAsync();
                    regionStats = stats.ToList();
                    _cache.Set("RegionStatistics", regionStats, TimeSpan.FromMinutes(10));
                }

                var mapData = regionStats
                    .Select(r =>
                    {
                        var mapId = RegionMapIdMapper.GetMapId(r.RegionId);
                        if (mapId == null) return null;

                        return new
                        {
                            regionId = r.RegionId,
                            regionName = r.Name,
                            regionNameRu = r.NameRu,
                            mapId = mapId,
                            totalVoters = r.TotalVoters,
                            lastUpdated = r.LastUpdated
                        };
                    })
                    .Where(x => x != null)
                    .ToList();

                return Json(mapData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetMapData");
                return Json(new List<object>());
            }
        }

        // Endpoint pentru testarea localităților
        [HttpGet]
        public async Task<IActionResult> TestLocalities(int regionId = 2)
        {
            try
            {
                _logger.LogInformation("TestLocalities called for RegionId={RegionId}", regionId);
                var results = await _statsRepository.GetLocalitiesByRegionAsync(regionId);
                var localities = results.ToList();
                
                return Json(new
                {
                    success = true,
                    regionId = regionId,
                    count = localities.Count,
                    localities = localities.Select(l => new
                    {
                        regionId = l.RegionId,
                        name = l.Name,
                        regionTypeId = l.RegionTypeId,
                        regionTypeName = l.RegionTypeName
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in TestLocalities for RegionId={RegionId}", regionId);
                return Json(new { success = false, error = ex.Message, stackTrace = ex.StackTrace });
            }
        }
    }
}
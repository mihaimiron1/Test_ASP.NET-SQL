    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;
    using MyApp.Models.Statistics;
    using MyApp.Repositories;
    using MyApp.Services;
    using System.Text.Json;

    namespace MyApp.Controllers
    {
        public class StatisticsController : Controller
        {
            private readonly IMemoryCache _cache;
            private readonly IStatisticsRepository _repository;
            private readonly ILogger<StatisticsController> _logger;

            public StatisticsController(
                IMemoryCache cache,
                IStatisticsRepository repository,
                ILogger<StatisticsController> logger)
            {
                _cache = cache;
                _repository = repository;
                _logger = logger;
            }

            // ============================================================
            // 1. HEAT MAP - Main page with map
            // ============================================================
            public async Task<IActionResult> HeatMap()
            {
                _logger.LogInformation("HeatMap requested at {time}", DateTime.Now);

                if (!_cache.TryGetValue("RegionStatistics", out IEnumerable<RegionVotingStatistic>? stats))
                {
                    _logger.LogWarning("Cache miss for RegionStatistics - fetching from database");
                    stats = await _repository.GetAllRegionsStatisticsAsync();
                    _cache.Set("RegionStatistics", stats, TimeSpan.FromSeconds(60));
                }
                else
                {
                    _logger.LogInformation("Cache hit for RegionStatistics");
                }

                // Prepare map data - only regions with valid MapId will be colored
                var mapData = (stats ?? Enumerable.Empty<RegionVotingStatistic>())
                    .Select(r =>
                    {
                        var mapId = RegionMapIdMapper.GetMapId(r.RegionId);
                        return new
                        {
                            MapId = mapId,
                            RegionId = r.RegionId,
                            RegionName = r.Name,
                            RegionNameRu = r.NameRu,
                            TotalVoters = r.TotalVoters,
                            LastUpdated = r.LastUpdated
                        };
                    })
                    .Where(r => !string.IsNullOrEmpty(r.MapId))
                    .ToList();

                _logger.LogInformation("HeatMap: Returning {count} regions with valid map IDs", mapData.Count);

                ViewBag.MapData = JsonSerializer.Serialize(mapData);
                return View();
            }

            // ============================================================
            // 2. REGION DETAILS - Click on region in map
            // ============================================================
            public async Task<IActionResult> RegionDetails(int regionId)
            {
                _logger.LogInformation("Region details requested for RegionId={regionId} at {time}", regionId, DateTime.Now);

                var cacheKey = $"RegionDetails_{regionId}";

                if (!_cache.TryGetValue(cacheKey, out DetailedRegionStatistic? stats))
                {
                    _logger.LogWarning("Cache miss for {cacheKey} - fetching from database", cacheKey);

                    try
                    {
                        stats = await _repository.GetRegionDetailedStatisticsAsync(regionId);

                        if (stats == null)
                        {
                            _logger.LogWarning("Region {regionId} not found", regionId);
                            return NotFound($"Regiunea cu ID {regionId} nu a fost găsită.");
                        }

                        _cache.Set(cacheKey, stats, TimeSpan.FromSeconds(60));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error fetching region details for {regionId}", regionId);
                        return BadRequest($"Eroare la citirea detaliilor pentru regiunea {regionId}.");
                    }
                }
                else
                {
                    _logger.LogInformation("Cache hit for {cacheKey}", cacheKey);
                }

                // Get localities for this region
                var localitiesCacheKey = $"Localities_{regionId}";
                if (!_cache.TryGetValue(localitiesCacheKey, out IEnumerable<RegionBasicInfo>? localities))
                {
                    try
                    {
                        localities = await _repository.GetLocalitiesByRegionAsync(regionId);
                        _cache.Set(localitiesCacheKey, localities, TimeSpan.FromSeconds(60));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error fetching localities for RegionId={regionId}", regionId);
                        localities = Enumerable.Empty<RegionBasicInfo>();
                    }
                }

                ViewBag.Localities = localities;
                return View(stats);
            }

            // ============================================================
            // 3. LOCALITY DETAILS - Selected from dropdown
            // ============================================================
            public async Task<IActionResult> LocalityDetails(int regionId)
            {
                _logger.LogInformation("Locality details requested for RegionId={regionId} at {time}", regionId, DateTime.Now);

                var cacheKey = $"LocalityDetails_{regionId}";

                if (!_cache.TryGetValue(cacheKey, out DetailedRegionStatistic? stats))
                {
                    _logger.LogWarning("Cache miss for {cacheKey} - fetching from database", cacheKey);

                    try
                    {
                        stats = await _repository.GetLocalityDetailedStatisticsAsync(regionId);

                        if (stats == null)
                        {
                            _logger.LogWarning("Locality {regionId} not found", regionId);
                            return NotFound($"Localitatea cu ID {regionId} nu a fost găsită.");
                        }

                        _cache.Set(cacheKey, stats, TimeSpan.FromSeconds(60));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error fetching locality details for {regionId}", regionId);
                        return BadRequest($"Eroare la citirea detaliilor pentru localitatea {regionId}.");
                    }
                }
                else
                {
                    _logger.LogInformation("Cache hit for {cacheKey}", cacheKey);
                }

                return View("RegionDetails", stats);
            }

            // ============================================================
            // 4. API - Get localities by region
            // ============================================================
            [HttpGet]
            public async Task<IActionResult> GetLocalitiesByRegionJson(int regionId)
            {
                var cacheKey = $"Localities_{regionId}";

                if (!_cache.TryGetValue(cacheKey, out IEnumerable<RegionBasicInfo>? localities))
                {
                    localities = await _repository.GetLocalitiesByRegionAsync(regionId);
                    _cache.Set(cacheKey, localities, TimeSpan.FromSeconds(60));
                }

                return Json(new
                {
                    success = true,
                    regionId = regionId,
                    localities = localities
                });
            }

            // ============================================================
            // 5. API JSON - AJAX refresh heat map
            // ============================================================
            [HttpGet]
            public async Task<IActionResult> GetHeatMapJson()
            {
                if (!_cache.TryGetValue("RegionStatistics", out IEnumerable<RegionVotingStatistic>? stats))
                {
                    stats = await _repository.GetAllRegionsStatisticsAsync();
                    _cache.Set("RegionStatistics", stats, TimeSpan.FromMinutes(5));
                }

                return Json(new
                {
                    success = true,
                    timestamp = DateTime.Now,
                    regions = stats
                });
            }

            // ============================================================
            // 6. API JSON - Region details (AJAX)
            // ============================================================
            [HttpGet]
            public async Task<IActionResult> GetRegionDetailsJson(int regionId)
            {
                var cacheKey = $"RegionDetails_{regionId}";

                if (!_cache.TryGetValue(cacheKey, out DetailedRegionStatistic? stats))
                {
                    try
                    {
                        stats = await _repository.GetRegionDetailedStatisticsAsync(regionId);

                        if (stats == null)
                        {
                            return Json(new { success = false, message = "Regiunea nu a fost găsită" });
                        }

                        _cache.Set(cacheKey, stats, TimeSpan.FromSeconds(60));
                    }
                    catch
                    {
                        return Json(new { success = false, message = "Eroare la citirea datelor" });
                    }
                }

                return Json(new
                {
                    success = true,
                    timestamp = DateTime.Now,
                    data = stats
                });
            }

            // ============================================================
            // 7. API JSON - Locality details (AJAX)
            // ============================================================
            [HttpGet]
            public async Task<IActionResult> GetLocalityDetailsJson(int regionId)
            {
                var cacheKey = $"LocalityDetails_{regionId}";

                if (!_cache.TryGetValue(cacheKey, out DetailedRegionStatistic? stats))
                {
                    try
                    {
                        stats = await _repository.GetLocalityDetailedStatisticsAsync(regionId);

                        if (stats == null)
                        {
                            return Json(new { success = false, message = "Localitatea nu a fost găsită" });
                        }

                        _cache.Set(cacheKey, stats, TimeSpan.FromSeconds(60));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error fetching locality details JSON for RegionId={regionId}", regionId);
                        return Json(new { success = false, message = $"Eroare: {ex.Message}" });
                    }
                }

                return Json(new
                {
                    success = true,
                    timestamp = DateTime.Now,
                    data = stats
                });
            }

            // ============================================================
            // 8. INFO - Cache status
            // ============================================================
            public IActionResult CacheInfo()
            {
                var hasRegionStats = _cache.TryGetValue("RegionStatistics", out IEnumerable<RegionVotingStatistic>? regionStats);

                var info = new
                {
                    RegionStatisticsCached = hasRegionStats,
                    RegionCount = hasRegionStats ? regionStats!.Count() : 0,
                    CurrentTime = DateTime.Now
                };

                return Json(info);
            }

            // ============================================================
            // 9. DEBUG - Verify map data and mappings
            // ============================================================
            [HttpGet]
            public async Task<IActionResult> DebugMapData()
            {
                var stats = await _repository.GetAllRegionsStatisticsAsync();

                var mapData = stats.Select(r => new
                {
                    DatabaseRegionId = r.RegionId,
                    DatabaseRegionName = r.Name,
                    DatabaseRegionNameRu = r.NameRu,
                    MapId = RegionMapIdMapper.GetMapId(r.RegionId),
                    TotalVoters = r.TotalVoters,
                    HasMapId = RegionMapIdMapper.HasMapping(r.RegionId),
                    MappingStatus = RegionMapIdMapper.HasMapping(r.RegionId)
                        ? "✅ Will be colored on map"
                        : "❌ NO MAPPING - Will be white"
                }).ToList();

                return Json(new
                {
                    Message = "Regions from database with mapping status",
                    TotalRegionsFromDatabase = mapData.Count,
                    RegionsWithMapId = mapData.Count(m => m.HasMapId),
                    RegionsWithoutMapId = mapData.Count(m => !m.HasMapId),
                    Note = "Regions without mapping will appear WHITE on the map",
                    Details = mapData
                });
            }

            // ============================================================
            // 10. API JSON - Region statistics for heat map click
            // ============================================================
            [HttpGet]
            public async Task<IActionResult> GetRegionStatisticsForHeatMap(int regionId)
            {
                _logger.LogInformation("Heat map region statistics requested for RegionId={regionId}", regionId);

                var cacheKey = $"HeatMapRegionStats_{regionId}";

                if (!_cache.TryGetValue(cacheKey, out DetailedRegionStatistic? stats))
                {
                    try
                    {
                        stats = await _repository.GetRegionDetailedStatisticsAsync(regionId);

                        if (stats == null)
                        {
                            return Json(new { success = false, message = "Regiunea nu a fost găsită" });
                        }

                        _cache.Set(cacheKey, stats, TimeSpan.FromSeconds(60));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error fetching heat map region statistics for RegionId={regionId}", regionId);
                        return Json(new { success = false, message = $"Eroare: {ex.Message}" });
                    }
                }

                return Json(new
                {
                    success = true,
                    regionId = stats.RegionId,
                    regionName = stats.RegionName,
                    totalVoters = stats.TotalVoters,
                    votedCount = stats.VotedCount,
                    votingPercentage = stats.VotingPercentage,
                    genderStats = stats.GenderStats.Select(g => new
                    {
                        gender = g.Gender,
                        voterCount = g.VoterCount,
                        votedCount = g.VotedCount,
                        percentage = g.Percentage,
                        color = g.Color
                    }),
                    ageStats = stats.AgeStats.Select(a => new
                    {
                        ageCategoryId = a.AgeCategoryId,
                        ageCategoryName = a.AgeCategoryName,
                        voterCount = a.VoterCount,
                        percentage = a.Percentage,
                        color = a.Color
                    }),
                    lastUpdated = stats.LastUpdated
                });
            }

            // ============================================================
            // 11. DIAGNOSTICS - Heat Map Troubleshooting Page
            // ============================================================
            public async Task<IActionResult> Diagnostics()
            {
                var stats = await _repository.GetAllRegionsStatisticsAsync();
                var mapData = (stats ?? Enumerable.Empty<RegionVotingStatistic>())
                    .Select(r =>
                    {
                        var mapId = RegionMapIdMapper.GetMapId(r.RegionId);
                        return new
                        {
                            MapId = mapId,
                            RegionId = r.RegionId,
                            RegionName = r.Name,
                            RegionNameRu = r.NameRu,
                            TotalVoters = r.TotalVoters,
                            LastUpdated = r.LastUpdated
                        };
                    })
                    .Where(r => !string.IsNullOrEmpty(r.MapId))
                    .ToList();

                ViewBag.MapData = JsonSerializer.Serialize(mapData);
                return View();
            }
        }
    }
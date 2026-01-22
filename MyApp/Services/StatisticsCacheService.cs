using Microsoft.Extensions.Caching.Memory;
using MyApp.Repositories;
using MyApp.Models.Statistics;

namespace MyApp.Services
{
    public class StatisticsCacheService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMemoryCache _cache;
        private readonly ILogger<StatisticsCacheService> _logger;

        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(30); // 30 seconds
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromSeconds(60); // 1 minute cache expiration

        public StatisticsCacheService(
            IServiceProvider serviceProvider,
            IMemoryCache cache,
            ILogger<StatisticsCacheService> logger)
        {
            _serviceProvider = serviceProvider;
            _cache = cache;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("📊 Statistics Cache Service STARTED at {time}", DateTime.Now);

            await UpdateStatisticsCache();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("⏰ Waiting {seconds} seconds until next update...", _updateInterval.TotalSeconds);

                    await Task.Delay(_updateInterval, stoppingToken);

                    await UpdateStatisticsCache();
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("🛑 Statistics Cache Service is stopping...");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ ERROR updating statistics cache");
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                }
            }

            _logger.LogInformation("🛑 Statistics Cache Service STOPPED at {time}", DateTime.Now);
        }

        private async Task UpdateStatisticsCache()
        {
            var startTime = DateTime.Now;
            _logger.LogInformation("🔄 Starting cache update at {time}", startTime);

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var statsRepository = scope.ServiceProvider.GetRequiredService<IStatisticsRepository>();

                // Actualizează statistici pentru HEAT MAP (doar regiunile cu cel puțin 1 vot)
                _logger.LogInformation("  → Updating region statistics for heat map...");
                var regionStats = await statsRepository.GetAllRegionsStatisticsAsync();
                var regionStatsList = regionStats.ToList();

                _cache.Set("RegionStatistics", regionStatsList, _cacheExpiration);
                _logger.LogInformation("  ✅ Cached {count} regions (with at least 1 vote from database)", regionStatsList.Count);

                // Pre-cache statistici pentru TOATE raioanele (gender + age)
                _logger.LogInformation("  → Pre-caching gender and age stats for all raions...");
                int successCount = 0;
                int failCount = 0;

                foreach (var region in regionStatsList)
                {
                    try
                    {
                        var detailedStats = await statsRepository.GetRegionDetailedStatisticsAsync(region.RegionId);
                        _cache.Set($"RegionDetails_{region.RegionId}", detailedStats, _cacheExpiration);
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "    ⚠️ Failed to pre-cache region {regionId} ({regionName})", 
                            region.RegionId, region.Name);
                        failCount++;
                    }
                }

                _logger.LogInformation("  ✅ Pre-cached {success} regions, {fail} failed", successCount, failCount);

                var duration = DateTime.Now - startTime;
                _logger.LogInformation("✅ Cache update COMPLETED in {seconds} seconds at {time}",
                    duration.TotalSeconds,
                    DateTime.Now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ FAILED to update statistics cache");
                // Important: don't crash the whole host if DB/stored procedures aren't ready yet.
                // We'll just log and try again on the next interval.
                return;
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("🛑 Statistics Cache Service is stopping gracefully...");
            await base.StopAsync(cancellationToken);
        }
    }
}
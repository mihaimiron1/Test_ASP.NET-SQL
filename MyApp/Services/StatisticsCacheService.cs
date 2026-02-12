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

        private readonly TimeSpan _updateInterval = TimeSpan.FromMinutes(10); // 10 minute
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(15); // 15 minute
        private readonly TimeSpan _initialDelay = TimeSpan.FromSeconds(10);

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
            _logger.LogInformation("📊 Cache Service STARTED");

            await Task.Delay(_initialDelay, stoppingToken);
            
            // Prima încărcare - DOAR lista de regiuni
            await UpdateRegionListCache();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_updateInterval, stoppingToken);
                    await UpdateRegionListCache();
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }

            _logger.LogInformation("🛑 Cache Service STOPPED");
        }

        private async Task UpdateRegionListCache()
        {
            try
            {
                var startTime = DateTime.Now;
                _logger.LogInformation("🔄 Updating region list cache");

                using var scope = _serviceProvider.CreateScope();
                var statsRepository = scope.ServiceProvider.GetRequiredService<IStatisticsRepository>();

                // Cache DOAR lista de regiuni (fără detalii)
                var regionStats = await statsRepository.GetAllRegionsStatisticsAsync();
                var regionStatsList = regionStats.ToList();
                _cache.Set("RegionStatistics", regionStatsList, _cacheExpiration);

                var duration = (DateTime.Now - startTime).TotalSeconds;
                _logger.LogInformation("✅ Cached {count} regions in {seconds:F1}s", 
                    regionStatsList.Count, duration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to update region list cache");
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("🛑 Cache Service stopping...");
            await base.StopAsync(cancellationToken);
        }
    }
}
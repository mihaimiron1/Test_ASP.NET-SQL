using MyApp.Models.Statistics;

namespace MyApp.Repositories
{
    public interface IStatisticsRepository
    {
        // Heat Map - toate raioanele
        Task<IEnumerable<RegionVotingStatistic>> GetAllRegionsStatisticsAsync();

        // Statistici detaliate pentru RAION
        Task<DetailedRegionStatistic> GetRegionDetailedStatisticsAsync(int regionId);

        // Statistici detaliate pentru LOCALITATE
        Task<DetailedRegionStatistic> GetLocalityDetailedStatisticsAsync(int regionId);

        // Helper - obține lista de localități dintr-un raion (pentru dropdown)
        Task<IEnumerable<RegionBasicInfo>> GetLocalitiesByRegionAsync(int regionId);
    }
}
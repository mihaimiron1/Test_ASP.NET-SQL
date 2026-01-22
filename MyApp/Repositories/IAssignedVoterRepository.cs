using MyApp.Models;

namespace MyApp.Repositories
{
    public interface IAssignedVoterRepository
    {
        Task<IEnumerable<RegionStatistic>> GetAllAsync();
        Task<IEnumerable<RegionStatistic>> GetByRegionAsync(int regionId);
        Task<IEnumerable<RegionStatistic>> GetByParentRegionAsync(int regionId);
    }
}   
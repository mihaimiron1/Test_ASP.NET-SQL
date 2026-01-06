using MyApp.Models;

namespace MyApp.Repositories
{
    public interface IAssignedVoterRepository
    {
        Task<IEnumerable<AssignedVoterStatistics>> GetAllAsync();
        Task<IEnumerable<AssignedVoterStatistics>> GetByRegionAsync(int regionId);
        Task<IEnumerable<AssignedVoterStatistics>> GetByParentRegionAsync(int regionId);
    }
}
using MyApp.Models.ElectionResult;

public interface IElectionResultRepository
{
    Task<IEnumerable<ElectionMunicipalityResultDto>> GetResultsByLocalityAsync(long electionId, long localityId);
    Task<IEnumerable<ElectionMunicipalityResultDto>> GetResultsByRaionAsync(long raionId);
    Task<IEnumerable<ElectionMunicipalityResultDto>> GetResultsByMunicipiuAsync(long electionId, long municipiuId);
    Task<IEnumerable<VotedRegionDto>> GetVotedRegionsByElectionIdAsync(long electionId);
    Task<IEnumerable<VotedLocalityDto>> GetVotedLocalitiesByRegionAsync(long electionId, long regionId);
}

using Microsoft.Data.SqlClient;
using System.Data;
using Dapper;
using MyApp.Models.ElectionResult;
namespace MyApp.Repositories
{
    public class ElectionResultRepository : IElectionResultRepository
    {
        private readonly string _connectionString;

        public ElectionResultRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<IEnumerable<ElectionMunicipalityResultDto>> GetResultsByLocalityAsync(long electionId, long localityId)
        {
            using var connection = new SqlConnection(_connectionString);
            var results = await connection.QueryAsync<ElectionMunicipalityResult>(
                "sp_ElectionResults_ByLocality_64_66",
                new { ElectionId = electionId, LocalityId = localityId },
                commandType: CommandType.StoredProcedure
            );
            return results.Select(r => new ElectionMunicipalityResultDto
            {
                MunicipiuId = r.MunicipiuId,
                MunicipiuName = r.MunicipiuName,
                PartyId = r.PartyId,
                PartyCode = r.PartyCode,
                PartyName = r.PartyName,
                ColorLogo = r.ColorLogo,
                CandidateMemberId = r.CandidateMemberId,
                CandidateFullName = r.CandidateFullName,
                Votes = r.Votes,
                TotalBallots = r.TotalBallots,
                ValidBallots = r.ValidBallots,
                SpoiledBallots = r.SpoiledBallots,
                UnusedSpoiledBallots = r.UnusedSpoiledBallots,
                CastedBallots = r.CastedBallots,
                RegisteredVoters = r.RegisteredVoters,
                Difference = r.Difference
            });
        }

        public async Task<IEnumerable<ElectionMunicipalityResultDto>> GetResultsByRaionAsync(long raionId)
        {
            using var connection = new SqlConnection(_connectionString);
            var results = await connection.QueryAsync<ElectionMunicipalityResult>(
                "sp_ElectionResults_ByRaion_10067",
                new { RaionId = raionId },
                commandType: CommandType.StoredProcedure
            );
            return results.Select(r => new ElectionMunicipalityResultDto
            {
                MunicipiuId = r.MunicipiuId,
                MunicipiuName = r.MunicipiuName,
                PartyId = r.PartyId,
                PartyCode = r.PartyCode,
                PartyName = r.PartyName,
                ColorLogo = r.ColorLogo,
                CandidateMemberId = r.CandidateMemberId,
                CandidateFullName = r.CandidateFullName,
                Votes = r.Votes,
                TotalBallots = r.TotalBallots,
                ValidBallots = r.ValidBallots,
                SpoiledBallots = r.SpoiledBallots,
                UnusedSpoiledBallots = r.UnusedSpoiledBallots,
                CastedBallots = r.CastedBallots,
                RegisteredVoters = r.RegisteredVoters,
                Difference = r.Difference
            });
        }

        public async Task<IEnumerable<ElectionMunicipalityResultDto>> GetResultsByMunicipiuAsync(long electionId, long municipiuId)
        {
            using var connection = new SqlConnection(_connectionString);
            var results = await connection.QueryAsync<ElectionMunicipalityResult>(
                "sp_ElectionResults_ByMunicipiu",
                new { ElectionId = electionId, MunicipiuId = municipiuId },
                commandType: CommandType.StoredProcedure
            );
            return results.Select(r => new ElectionMunicipalityResultDto
            {
                MunicipiuId = r.MunicipiuId,
                MunicipiuName = r.MunicipiuName,
                PartyId = r.PartyId,
                PartyCode = r.PartyCode,
                PartyName = r.PartyName,
                ColorLogo = r.ColorLogo,
                CandidateMemberId = r.CandidateMemberId,
                CandidateFullName = r.CandidateFullName,
                Votes = r.Votes,
                TotalBallots = r.TotalBallots,
                ValidBallots = r.ValidBallots,
                SpoiledBallots = r.SpoiledBallots,
                UnusedSpoiledBallots = r.UnusedSpoiledBallots,
                CastedBallots = r.CastedBallots,
                RegisteredVoters = r.RegisteredVoters,
                Difference = r.Difference
            });
        }
    }
}

using Microsoft.Data.SqlClient;
using System.Data;
using Dapper;
using MyApp.Models.ElectionResult;
namespace MyApp.Repositories
{
    public class ElectionResultRepository : IElectionResultRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<ElectionResultRepository> _logger;
        private const int CommandTimeoutSeconds = 180;

        public ElectionResultRepository(IConfiguration configuration, ILogger<ElectionResultRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
            _logger = logger;
        }

        public async Task<IEnumerable<ElectionMunicipalityResultDto>> GetResultsByLocalityAsync(long electionId, long localityId)
        {
            using var connection = new SqlConnection(_connectionString);
            var results = await connection.QueryAsync<ElectionMunicipalityResult>(
                "sp_ElectionResults_ByLocality_64_66",
                new { ElectionId = electionId, LocalityId = localityId },
                commandType: CommandType.StoredProcedure,
                commandTimeout: CommandTimeoutSeconds
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
            _logger.LogInformation("Calling sp_ElectionResults_ByRaion_10067 for RaionId={RaionId} (timeout={TimeoutSeconds}s)...", raionId, CommandTimeoutSeconds);
            using var connection = new SqlConnection(_connectionString);

            // NOTE:
            // sp_ElectionResults_ByRaion_10067 may return columns like ColorLogo as VARBINARY/IMAGE.
            // Mapping to ElectionMunicipalityResult (where ColorLogo is string) can throw.
            // For the Test page we only need PartyCode/PartyName/Votes, so we map to a minimal type
            // and ignore the rest of the columns.
            var results = await connection.QueryAsync<RaionPartyVotesRow>(
                "sp_ElectionResults_ByRaion_10067",
                new { RaionId = raionId },
                commandType: CommandType.StoredProcedure,
                commandTimeout: CommandTimeoutSeconds
            );

            return results.Select(r => new ElectionMunicipalityResultDto
            {
                // Only fields used by Test endpoint
                PartyCode = r.PartyCode ?? "",
                PartyName = r.PartyName ?? "",
                Votes = r.Votes,

                // Fill required DTO fields with safe defaults
                MunicipiuId = 0,
                MunicipiuName = "",
                PartyId = 0,
                ColorLogo = "",
                CandidateMemberId = 0,
                CandidateFullName = "",
                TotalBallots = 0,
                ValidBallots = 0,
                SpoiledBallots = 0,
                UnusedSpoiledBallots = 0,
                CastedBallots = 0,
                RegisteredVoters = 0,
                Difference = 0
            });
        }

        public async Task<IEnumerable<ElectionMunicipalityResultDto>> GetResultsByMunicipiuAsync(long electionId, long municipiuId)
        {
            using var connection = new SqlConnection(_connectionString);
            var results = await connection.QueryAsync<ElectionMunicipalityResult>(
                "sp_ElectionResults_ByMunicipiu",
                new { ElectionId = electionId, MunicipiuId = municipiuId },
                commandType: CommandType.StoredProcedure,
                commandTimeout: CommandTimeoutSeconds
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

        private sealed class RaionPartyVotesRow
        {
            public string? PartyCode { get; set; }
            public string? PartyName { get; set; }
            public int Votes { get; set; }
        }
    }
}

using Dapper;
using Microsoft.Data.SqlClient;
using MyApp.Models.ElectionResult;
using System.Data;
using System.Data.SqlTypes;

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
            _logger.LogInformation("Calling sp_ElectionResults_ByLocality_64_66 for ElectionId={ElectionId}, LocalityId={LocalityId} (timeout={TimeoutSeconds}s)...", electionId, localityId, CommandTimeoutSeconds);
            using var connection = new SqlConnection(_connectionString);

            var rows = (await connection.QueryAsync(
                "sp_ElectionResults_ByLocality_64_66",
                new { ElectionId = electionId, LocalityId = localityId },
                commandType: CommandType.StoredProcedure,
                commandTimeout: CommandTimeoutSeconds
            )).ToList();

            var mapped = rows.Select(r =>
            {
                var row = (IDictionary<string, object?>)r;

                var logoRaw = GetFirstAvailableValue(row,
                    "ColorLogo", "PartyLogo", "Logo", "Color", "LogoColor", "PartyColor");

                if (logoRaw == null)
                {
                    var guessedLogoColumn = row.Keys.FirstOrDefault(k =>
                        k.Contains("logo", StringComparison.OrdinalIgnoreCase) ||
                        k.Contains("color", StringComparison.OrdinalIgnoreCase));

                    if (!string.IsNullOrWhiteSpace(guessedLogoColumn) && row.TryGetValue(guessedLogoColumn, out var guessedValue))
                    {
                        logoRaw = guessedValue;
                    }
                }

                return new ElectionMunicipalityResultDto
                {
                    PartyCode = GetStringValue(row, "PartyCode") ?? string.Empty,
                    PartyName = GetStringValue(row, "PartyName") ?? string.Empty,
                    CandidateFullName = GetStringValue(row, "CandidateFullName") ?? string.Empty,
                    ColorLogo = ConvertColorLogoToImageSource(logoRaw) ?? string.Empty,
                    Votes = GetIntValue(row, "Votes"),

                    MunicipiuId = 0,
                    MunicipiuName = string.Empty,
                    PartyId = 0,
                    CandidateMemberId = 0,
                    TotalBallots = 0,
                    ValidBallots = 0,
                    SpoiledBallots = 0,
                    UnusedSpoiledBallots = 0,
                    CastedBallots = 0,
                    RegisteredVoters = 0,
                    Difference = 0
                };
            }).ToList();

            _logger.LogInformation("Locality results mapped for ElectionId={ElectionId}, LocalityId={LocalityId}. Rows={Rows}, Logos={Logos}",
                electionId, localityId, mapped.Count, mapped.Count(x => !string.IsNullOrWhiteSpace(x.ColorLogo)));

            return mapped;
        }

        public async Task<IEnumerable<ElectionMunicipalityResultDto>> GetResultsByRaionAsync(long raionId)
        {
            _logger.LogInformation("Calling sp_ElectionResults_ByRaion_10067 for RaionId={RaionId} (timeout={TimeoutSeconds}s)...", raionId, CommandTimeoutSeconds);
            using var connection = new SqlConnection(_connectionString);

            var rows = (await connection.QueryAsync(
                "sp_ElectionResults_ByRaion_10067",
                new { RaionId = raionId },
                commandType: CommandType.StoredProcedure,
                commandTimeout: CommandTimeoutSeconds
            )).ToList();

            var mapped = rows.Select(r =>
            {
                var row = (IDictionary<string, object?>)r;
                var logoRaw = GetFirstAvailableValue(row,
                    "ColorLogo", "PartyLogo", "Logo", "Color", "LogoColor", "PartyColor");

                if (logoRaw == null)
                {
                    var guessedLogoColumn = row.Keys.FirstOrDefault(k =>
                        k.Contains("logo", StringComparison.OrdinalIgnoreCase) ||
                        k.Contains("color", StringComparison.OrdinalIgnoreCase));

                    if (!string.IsNullOrWhiteSpace(guessedLogoColumn) && row.TryGetValue(guessedLogoColumn, out var guessedValue))
                    {
                        logoRaw = guessedValue;
                    }
                }

                return new ElectionMunicipalityResultDto
                {
                    PartyCode = GetStringValue(row, "PartyCode") ?? string.Empty,
                    PartyName = GetStringValue(row, "PartyName") ?? string.Empty,
                    Votes = GetIntValue(row, "Votes"),

                    MunicipiuId = 0,
                    MunicipiuName = string.Empty,
                    PartyId = 0,
                    ColorLogo = ConvertColorLogoToImageSource(logoRaw) ?? string.Empty,
                    CandidateMemberId = 0,
                    CandidateFullName = string.Empty,
                    TotalBallots = 0,
                    ValidBallots = 0,
                    SpoiledBallots = 0,
                    UnusedSpoiledBallots = 0,
                    CastedBallots = 0,
                    RegisteredVoters = 0,
                    Difference = 0
                };
            }).ToList();

            _logger.LogInformation("Raion results mapped for RaionId={RaionId}. Rows={Rows}, Logos={Logos}",
                raionId, mapped.Count, mapped.Count(x => !string.IsNullOrWhiteSpace(x.ColorLogo)));

            return mapped;
        }

        public async Task<IEnumerable<ElectionMunicipalityResultDto>> GetResultsByMunicipiuAsync(long electionId, long municipiuId)
        {
            _logger.LogInformation("Calling sp_ElectionResults_ByMunicipiu for ElectionId={ElectionId}, MunicipiuId={MunicipiuId} (timeout={TimeoutSeconds}s)...", electionId, municipiuId, CommandTimeoutSeconds);
            using var connection = new SqlConnection(_connectionString);

            var results = await connection.QueryAsync<MunicipiuCandidateVotesRow>(
                "sp_ElectionResults_ByMunicipiu",
                new { ElectionId = electionId, MunicipiuId = municipiuId },
                commandType: CommandType.StoredProcedure,
                commandTimeout: CommandTimeoutSeconds
            );

            return results.Select(r => new ElectionMunicipalityResultDto
            {
                PartyCode = r.PartyCode ?? string.Empty,
                PartyName = r.PartyName ?? string.Empty,
                CandidateFullName = r.CandidateFullName ?? string.Empty,
                ColorLogo = ConvertColorLogoToImageSource(r.GetAnyLogoValue()) ?? string.Empty,
                Votes = r.Votes,

                MunicipiuId = 0,
                MunicipiuName = string.Empty,
                PartyId = 0,
                CandidateMemberId = 0,
                TotalBallots = 0,
                ValidBallots = 0,
                SpoiledBallots = 0,
                UnusedSpoiledBallots = 0,
                CastedBallots = 0,
                RegisteredVoters = 0,
                Difference = 0
            });
        }

        public async Task<IEnumerable<VotedRegionDto>> GetVotedRegionsByElectionIdAsync(long electionId)
        {
            _logger.LogInformation("Calling sp_GetVotedRegionsByElectionId for ElectionId={ElectionId} (timeout={TimeoutSeconds}s)...", electionId, CommandTimeoutSeconds);
            using var connection = new SqlConnection(_connectionString);

            var results = await connection.QueryAsync<VotedRegionDto>(
                "sp_GetVotedRegionsByElectionId",
                new { ElectionId = electionId },
                commandType: CommandType.StoredProcedure,
                commandTimeout: CommandTimeoutSeconds
            );

            return results;
        }

        public async Task<IEnumerable<VotedLocalityDto>> GetVotedLocalitiesByRegionAsync(long electionId, long regionId)
        {
            _logger.LogInformation("Calling sp_GetVotedLocalitiesByRegion for ElectionId={ElectionId}, RegionId={RegionId} (timeout={TimeoutSeconds}s)...", electionId, regionId, CommandTimeoutSeconds);
            using var connection = new SqlConnection(_connectionString);

            var results = await connection.QueryAsync<VotedLocalityDto>(
                "sp_GetVotedLocalitiesByRegion",
                new { ElectionId = electionId, RegionId = regionId },
                commandType: CommandType.StoredProcedure,
                commandTimeout: CommandTimeoutSeconds
            );

            return results;
        }

        private static string? ConvertColorLogoToImageSource(object? colorLogo)
        {
            if (colorLogo == null) return null;

            if (colorLogo is byte[] logoBytes && logoBytes.Length > 0)
            {
                return $"data:image/png;base64,{Convert.ToBase64String(logoBytes)}";
            }

            if (colorLogo is SqlBinary sqlBinary && !sqlBinary.IsNull && sqlBinary.Length > 0)
            {
                return $"data:image/png;base64,{Convert.ToBase64String(sqlBinary.Value)}";
            }

            if (colorLogo is SqlBytes sqlBytes && !sqlBytes.IsNull && sqlBytes.Length > 0)
            {
                var value = sqlBytes.Value;
                return value is { Length: > 0 }
                    ? $"data:image/png;base64,{Convert.ToBase64String(value)}"
                    : null;
            }

            if (colorLogo is string logoText && !string.IsNullOrWhiteSpace(logoText))
            {
                var trimmed = logoText.Trim();

                if (IsCssColorValue(trimmed))
                {
                    return CreateColorSwatchDataUri(trimmed);
                }

                if (IsHexColorWithoutHash(trimmed))
                {
                    return CreateColorSwatchDataUri("#" + trimmed);
                }

                if (IsLikelyCssColorName(trimmed))
                {
                    return CreateColorSwatchDataUri(trimmed);
                }

                if (trimmed.StartsWith("data:image", StringComparison.OrdinalIgnoreCase) ||
                    trimmed.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                    trimmed.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
                    trimmed.StartsWith("/"))
                {
                    return trimmed;
                }

                if (trimmed.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                {
                    var hex = trimmed[2..];
                    if (TryHexToBytes(hex, out var bytes) && bytes.Length > 0)
                    {
                        return $"data:image/png;base64,{Convert.ToBase64String(bytes)}";
                    }
                }

                if (trimmed.StartsWith("<svg", StringComparison.OrdinalIgnoreCase))
                {
                    return $"data:image/svg+xml;base64,{Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(trimmed))}";
                }

                return $"data:image/png;base64,{trimmed}";
            }

            return null;
        }

        private static bool IsCssColorValue(string value)
        {
            return value.StartsWith("#", StringComparison.Ordinal)
                || value.StartsWith("rgb(", StringComparison.OrdinalIgnoreCase)
                || value.StartsWith("rgba(", StringComparison.OrdinalIgnoreCase)
                || value.StartsWith("hsl(", StringComparison.OrdinalIgnoreCase)
                || value.StartsWith("hsla(", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsHexColorWithoutHash(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            if (value.Length is not (3 or 6 or 8)) return false;

            foreach (var c in value)
            {
                var isHex = (c >= '0' && c <= '9') ||
                            (c >= 'a' && c <= 'f') ||
                            (c >= 'A' && c <= 'F');
                if (!isHex) return false;
            }

            return true;
        }

        private static bool IsLikelyCssColorName(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length > 20) return false;
            return value.All(ch => char.IsLetter(ch) || ch == '-');
        }

        private static string CreateColorSwatchDataUri(string cssColor)
        {
            var safeColor = System.Security.SecurityElement.Escape(cssColor) ?? "#9ca3af";
            var svg = $"<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 32 32'><rect x='1' y='1' width='30' height='30' rx='6' ry='6' fill='{safeColor}' stroke='#d1d5db' stroke-width='1'/></svg>";
            return $"data:image/svg+xml;base64,{Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(svg))}";
        }

        private static bool TryHexToBytes(string hex, out byte[] bytes)
        {
            bytes = Array.Empty<byte>();
            if (string.IsNullOrWhiteSpace(hex) || hex.Length % 2 != 0) return false;

            try
            {
                var result = new byte[hex.Length / 2];
                for (var i = 0; i < result.Length; i++)
                {
                    result[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
                }

                bytes = result;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private sealed class MunicipiuCandidateVotesRow
        {
            public string? PartyCode { get; set; }
            public string? PartyName { get; set; }
            public string? CandidateFullName { get; set; }
            public object? ColorLogo { get; set; }
            public object? PartyLogo { get; set; }
            public object? Logo { get; set; }
            public int Votes { get; set; }

            public object? GetAnyLogoValue()
            {
                return ColorLogo ?? PartyLogo ?? Logo;
            }
        }

        private sealed class LocalityCandidateVotesRow
        {
            public string? PartyCode { get; set; }
            public string? PartyName { get; set; }
            public string? CandidateFullName { get; set; }
            public object? ColorLogo { get; set; }
            public object? PartyLogo { get; set; }
            public object? Logo { get; set; }
            public int Votes { get; set; }

            public object? GetAnyLogoValue()
            {
                return ColorLogo ?? PartyLogo ?? Logo;
            }
        }

        private sealed class RaionPartyVotesRow
        {
            public string? PartyCode { get; set; }
            public string? PartyName { get; set; }
            public int Votes { get; set; }
        }

        private static object? GetFirstAvailableValue(IDictionary<string, object?> row, params string[] keys)
        {
            foreach (var key in keys)
            {
                if (row.TryGetValue(key, out var value) && value != null && value != DBNull.Value)
                {
                    return value;
                }

                var match = row.Keys.FirstOrDefault(k => string.Equals(k, key, StringComparison.OrdinalIgnoreCase));
                if (match != null && row.TryGetValue(match, out value) && value != null && value != DBNull.Value)
                {
                    return value;
                }
            }

            return null;
        }

        private static string? GetStringValue(IDictionary<string, object?> row, string key)
        {
            var value = GetFirstAvailableValue(row, key);
            return value?.ToString();
        }

        private static int GetIntValue(IDictionary<string, object?> row, string key)
        {
            var value = GetFirstAvailableValue(row, key);
            if (value == null) return 0;

            if (value is int i) return i;
            if (value is long l) return (int)l;
            if (int.TryParse(value.ToString(), out var parsed)) return parsed;
            return 0;
        }
    }
}

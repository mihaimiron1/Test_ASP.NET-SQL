using Microsoft.AspNetCore.Mvc;
using MyApp.Repositories;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace MyApp.Controllers
{
    public class RezultateController : Controller
    {
        private readonly IElectionResultRepository _electionResultRepository;
        private readonly ILogger<RezultateController> _logger;

        public RezultateController(IElectionResultRepository electionResultRepository, ILogger<RezultateController> logger)
        {
            _electionResultRepository = electionResultRepository;
            _logger = logger;
        }

        public IActionResult Index() => View();
        public IActionResult Statistici() => View();
        public IActionResult ConsilieriMunicipali() => View();
        public IActionResult ConsilieriLocali() => View();
        public IActionResult ConsilieriRaionali() => View();
        public IActionResult PrimariLocali() => View();
        public IActionResult PrimariMunicipali() => View();
        public IActionResult Test() => View();

        [HttpGet]
        public async Task<IActionResult> GetElectionResultsByRaion(long raionId)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var results = await _electionResultRepository.GetResultsByRaionAsync(raionId);
                var mappedResults = results.OrderByDescending(r => r.Votes).Select(r => new
                {
                    partyCode = r.PartyCode,
                    partyName = r.PartyName,
                    colorLogo = r.ColorLogo,
                    votes = r.Votes
                }).ToList();

                _logger.LogInformation("GetElectionResultsByRaion RaionId={RaionId} completed in {ElapsedMs}ms. Rows={Rows}",
                    raionId, sw.ElapsedMilliseconds, mappedResults.Count);
                return Json(new { success = true, results = mappedResults });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetElectionResultsByRaion failed for RaionId={RaionId} after {ElapsedMs}ms", raionId, sw.ElapsedMilliseconds);
                return Json(new { success = false, message = "Eroare la înc?rcarea rezultatelor: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetElectionResultsByMunicipiu(long municipiuId)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                const long electionId = 10069;
                var results = await _electionResultRepository.GetResultsByMunicipiuAsync(electionId, municipiuId);
                var mappedResults = results.OrderByDescending(r => r.Votes).Select(r => new
                {
                    partyCode = r.PartyCode,
                    partyName = r.PartyName,
                    candidateName = r.CandidateFullName,
                    colorLogo = r.ColorLogo,
                    votes = r.Votes
                }).ToList();

                _logger.LogInformation("GetElectionResultsByMunicipiu MunicipiuId={MunicipiuId} completed in {ElapsedMs}ms. Rows={Rows}, Logos={Logos}",
                    municipiuId, sw.ElapsedMilliseconds, mappedResults.Count, mappedResults.Count(x => !string.IsNullOrWhiteSpace(x.colorLogo)));

                return Json(new { success = true, results = mappedResults });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetElectionResultsByMunicipiu failed for MunicipiuId={MunicipiuId} after {ElapsedMs}ms", municipiuId, sw.ElapsedMilliseconds);
                return Json(new { success = false, message = "Eroare la înc?rcarea rezultatelor: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetElectionResultsByMunicipiuConsilieri(long municipiuId)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                const long electionId = 10068;
                var results = await _electionResultRepository.GetResultsByMunicipiuAsync(electionId, municipiuId);
                var mappedResults = results.OrderByDescending(r => r.Votes).Select(r => new
                {
                    partyCode = r.PartyCode,
                    partyName = r.PartyName,
                    candidateName = r.CandidateFullName,
                    colorLogo = r.ColorLogo,
                    votes = r.Votes
                }).ToList();

                return Json(new { success = true, results = mappedResults });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetElectionResultsByMunicipiuConsilieri failed for MunicipiuId={MunicipiuId} after {ElapsedMs}ms", municipiuId, sw.ElapsedMilliseconds);
                return Json(new { success = false, message = "Eroare la înc?rcarea rezultatelor: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetElectionResultsByLocalityPrimari(long localityId)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                const long electionId = 10064;
                var results = await _electionResultRepository.GetResultsByLocalityAsync(electionId, localityId);
                var mappedResults = results.OrderByDescending(r => r.Votes).Select(r => new
                {
                    partyCode = r.PartyCode,
                    candidateName = r.CandidateFullName,
                    colorLogo = r.ColorLogo,
                    votes = r.Votes
                }).ToList();

                _logger.LogInformation("GetElectionResultsByLocalityPrimari LocalityId={LocalityId} completed in {ElapsedMs}ms. Rows={Rows}, Logos={Logos}",
                    localityId, sw.ElapsedMilliseconds, mappedResults.Count, mappedResults.Count(x => !string.IsNullOrWhiteSpace(x.colorLogo)));

                return Json(new { success = true, results = mappedResults });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetElectionResultsByLocalityPrimari failed for LocalityId={LocalityId} after {ElapsedMs}ms", localityId, sw.ElapsedMilliseconds);
                return Json(new { success = false, message = "Eroare la înc?rcarea rezultatelor localit??ii: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetElectionResultsByLocalityConsilieri(long localityId)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                const long electionId = 10066;
                var results = await _electionResultRepository.GetResultsByLocalityAsync(electionId, localityId);
                var mappedResults = results.OrderByDescending(r => r.Votes).Select(r => new
                {
                    partyCode = r.PartyCode,
                    candidateName = r.CandidateFullName,
                    colorLogo = r.ColorLogo,
                    votes = r.Votes
                }).ToList();

                _logger.LogInformation("GetElectionResultsByLocalityConsilieri LocalityId={LocalityId} completed in {ElapsedMs}ms. Rows={Rows}, Logos={Logos}",
                    localityId, sw.ElapsedMilliseconds, mappedResults.Count, mappedResults.Count(x => !string.IsNullOrWhiteSpace(x.colorLogo)));

                return Json(new { success = true, results = mappedResults });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetElectionResultsByLocalityConsilieri failed for LocalityId={LocalityId} after {ElapsedMs}ms", localityId, sw.ElapsedMilliseconds);
                return Json(new { success = false, message = "Eroare la înc?rcarea rezultatelor localit??ii: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetVotedRegionsByElectionId(long electionId)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var regions = await _electionResultRepository.GetVotedRegionsByElectionIdAsync(electionId);

                var mapped = regions
                    .Select(r =>
                    {
                        var mapId = Services.RegionMapIdMapper.GetMapId(r.RegionId) ?? TryMapByRegionName(r.RegionName);
                        if (mapId == null) return null;

                        return new
                        {
                            regionId = r.RegionId,
                            regionName = r.RegionName,
                            mapId = mapId
                        };
                    })
                    .Where(x => x != null)
                    .ToList();

                _logger.LogInformation("GetVotedRegionsByElectionId ElectionId={ElectionId} completed in {ElapsedMs}ms. Regions={Count}",
                    electionId, sw.ElapsedMilliseconds, mapped.Count);

                return Json(mapped);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetVotedRegionsByElectionId failed for ElectionId={ElectionId} after {ElapsedMs}ms", electionId, sw.ElapsedMilliseconds);
                return Json(new List<object>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetVotedLocalitiesByRegion(long electionId, long regionId)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var localities = await _electionResultRepository.GetVotedLocalitiesByRegionAsync(electionId, regionId);

                var result = localities.Select(l => new
                {
                    localityId = l.LocalityId,
                    localityName = l.LocalityName
                }).ToList();

                _logger.LogInformation("GetVotedLocalitiesByRegion ElectionId={ElectionId}, RegionId={RegionId} completed in {ElapsedMs}ms. Localities={Count}",
                    electionId, regionId, sw.ElapsedMilliseconds, result.Count);

                return Json(new { success = true, localities = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetVotedLocalitiesByRegion failed for ElectionId={ElectionId}, RegionId={RegionId} after {ElapsedMs}ms", electionId, regionId, sw.ElapsedMilliseconds);
                return Json(new { success = false, message = "Eroare la înc?rcarea localit??ilor: " + ex.Message });
            }
        }

        private static string? TryMapByRegionName(string? regionName)
        {
            var key = NormalizeRegionName(regionName);
            return key switch
            {
                "anenii noi" => "MD-AN",
                "basarabeasca" => "MD-BS",
                "balti" => "MD-BA",
                "briceni" => "MD-BR",
                "cahul" => "MD-CA",
                "cantemir" => "MD-CT",
                "calarasi" => "MD-CL",
                "causeni" => "MD-CS",
                "chisinau" => "MD-CU",
                "cimislia" => "MD-CM",
                "criuleni" => "MD-CR",
                "donduseni" => "MD-DO",
                "drochia" => "MD-DR",
                "dubasari" => "MD-DU",
                "edinet" => "MD-ED",
                "falesti" => "MD-FA",
                "floresti" => "MD-FL",
                "gagauzia" => "MD-GA",
                "uta gagauzia" => "MD-GA",
                "glodeni" => "MD-GL",
                "hincesti" => "MD-HI",
                "ialoveni" => "MD-IA",
                "leova" => "MD-LE",
                "nisporeni" => "MD-NI",
                "ocnita" => "MD-OC",
                "orhei" => "MD-OR",
                "rezina" => "MD-RE",
                "riscani" => "MD-RI",
                "singerei" => "MD-SI",
                "soldanesti" => "MD-SD",
                "soroca" => "MD-SO",
                "stefan voda" => "MD-SV",
                "straseni" => "MD-ST",
                "taraclia" => "MD-TA",
                "telenesti" => "MD-TE",
                "ungheni" => "MD-UN",
                "unitatea teritoriala din stinga nistrului" => "MD-SN",
                _ => null
            };
        }

        private static string NormalizeRegionName(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;

            var normalized = value.ToLowerInvariant().Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(normalized.Length);

            foreach (var ch in normalized)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (category == UnicodeCategory.NonSpacingMark) continue;
                sb.Append(ch);
            }

            return sb.ToString().Normalize(NormalizationForm.FormC).Trim();
        }
    }
}

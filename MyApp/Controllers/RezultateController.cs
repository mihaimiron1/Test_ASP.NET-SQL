using Microsoft.AspNetCore.Mvc;
using MyApp.Models;
using MyApp.Repositories;
using System.Diagnostics;
using System.Text.Json;

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
            

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Statistici()
        {
        return View();
        }

        public IActionResult ConsilieriMunicipali()
        {
            return View();
        }
        public IActionResult ConsilieriLocali()
        {
            return View();
        }

        public IActionResult ConsilieriRaionali()
        {
            return View();
        }

        public IActionResult PrimariLocali()
        {
            return View();
        }
        public IActionResult PrimariMunicipali()
        {
            return View();
        }
        public IActionResult Test()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetElectionResultsByRaion(long raionId)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                _logger.LogInformation("GetElectionResultsByRaion called with RaionId={RaionId}", raionId);
                var results = await _electionResultRepository.GetResultsByRaionAsync(raionId);

                var top7 = results
                    .OrderByDescending(r => r.Votes)
                    .Take(7)
                    .Select(r => new
                    {
                        partyCode = r.PartyCode,
                        partyName = r.PartyName,
                        votes = r.Votes
                    })
                    .ToList();

                _logger.LogInformation("GetElectionResultsByRaion RaionId={RaionId} completed in {ElapsedMs}ms. Rows={Rows}, Top7={Top7}",
                    raionId, sw.ElapsedMilliseconds, results.Count(), top7.Count);
                return Json(new { success = true, results = top7 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetElectionResultsByRaion failed for RaionId={RaionId} after {ElapsedMs}ms", raionId, sw.ElapsedMilliseconds);
                return Json(new { success = false, message = "Eroare la încărcarea rezultatelor: " + ex.Message });
            }
        }
    }
}

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

        public RezultateController(IElectionResultRepository electionResultRepository)
        {
            _electionResultRepository = electionResultRepository;
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
            try
            {
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

                return Json(new { success = true, results = top7 });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Eroare la înc?rcarea rezultatelor: " + ex.Message });
            }
        }
    }
}

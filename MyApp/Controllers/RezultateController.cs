using Microsoft.AspNetCore.Mvc;
using MyApp.Models;
using System.Diagnostics;
using System.Text.Json;

namespace MyApp.Controllers
{
    public class RezultateController : Controller
    {


        public IActionResult Index()
        {
            return View();
        }

























        public IActionResult Statistici()
        {
            // Prezența la vot (procente)
            ViewBag.Prezenta = 80;

            // Participare pe gen (procente)
            var genderData = new[]
            {
                new { Label = "Femei", Value = 60 },
                new { Label = "Bărbați", Value = 40 }
            };

            var ageData = new[]
            {
                new { Group = "18-25", Percentage = 42 },
                new { Group = "26-35", Percentage = 58 },
                new { Group = "36-45", Percentage = 64 },
                new { Group = "46-55", Percentage = 71 },
                new { Group = "56-65", Percentage = 78 },
                new { Group = "65+", Percentage = 82 }
            };

            // Date pentru hartă - raioanele cu statistici electorale
            var mapData = new[]
            {
                new {
                    Id = "MD-AN", // Anenii Noi
                    Raion = "Anenii Noi",
                    ProcenteVot = 75.5,
                    ProcenteFemei = 58.2,
                    ProcenteBarbati = 41.8
                },
                new {
                    Id = "MD-BS", // Bălți
                    Raion = "Bălți",
                    ProcenteVot = 82.3,
                    ProcenteFemei = 62.1,
                    ProcenteBarbati = 37.9
                },
                new {
                    Id = "MD-CA", // Cahul
                    Raion = "Cahul",
                    ProcenteVot = 78.9,
                    ProcenteFemei = 55.4,
                    ProcenteBarbati = 44.6
                },
                new {
                    Id = "MD-CU", // Chișinău
                    Raion = "Chișinău",
                    ProcenteVot = 85.7,
                    ProcenteFemei = 61.3,
                    ProcenteBarbati = 38.7
                },
                new {
                    Id = "MD-ED", // Edineț
                    Raion = "Edineț",
                    ProcenteVot = 72.4,
                    ProcenteFemei = 57.8,
                    ProcenteBarbati = 42.2
                },
                new {
                    Id = "MD-OR", // Orhei
                    Raion = "Orhei",
                    ProcenteVot = 79.1,
                    ProcenteFemei = 59.6,
                    ProcenteBarbati = 40.4
                },
                new {
                    Id = "MD-SO", // Soroca
                    Raion = "Soroca",
                    ProcenteVot = 76.8,
                    ProcenteFemei = 60.2,
                    ProcenteBarbati = 39.8
                },
                new {
                    Id = "MD-UN", // Ungheni
                    Raion = "Ungheni",
                    ProcenteVot = 81.2,
                    ProcenteFemei = 56.9,
                    ProcenteBarbati = 43.1
                }
            };

            ViewBag.GenderData = JsonSerializer.Serialize(genderData);
            ViewBag.AgeData = JsonSerializer.Serialize(ageData);
            ViewBag.MapData = JsonSerializer.Serialize(mapData);

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

        public IActionResult PrimariMunicipali()
        {
            return View();
        }

        public IActionResult PrimariLocali()
        {
            return View();
        }
        public IActionResult Test()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

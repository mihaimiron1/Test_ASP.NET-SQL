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

            ViewBag.GenderData = JsonSerializer.Serialize(genderData);
            ViewBag.AgeData = JsonSerializer.Serialize(ageData);
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


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

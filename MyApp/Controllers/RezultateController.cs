using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyApp.Models;

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

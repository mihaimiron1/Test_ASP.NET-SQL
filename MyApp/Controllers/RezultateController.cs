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
            var mapData = new[]
            {
                new {
                    Id = "MD-AN",
                    Raion = "Anenii Noi",
                    ProcenteVot = 68.4,
                    ProcenteFemei = 54.1,
                    ProcenteBarbati = 45.9,
                    Votanti = 45210,
                    Partid = "PAS", // castigator
                    PartidLogo = "/images/default.png",
                    Candidat = "Ion Popescu",
                    Candidates = new[] {
                        new { Candidat = "Ion Popescu", Partid = "PAS", PartidLogo = "/images/default.png", ProcenteVot = 38.5 },
                        new { Candidat = "Vasile Corneanu", Partid = "PSRM", PartidLogo = "/images/default.png", ProcenteVot = 26.1 },
                        new { Candidat = "Maria Țîrdea", Partid = "PPDA", PartidLogo = "/images/default.png", ProcenteVot = 14.7 },
                        new { Candidat = "Mihai Grigoraș", Partid = "MAN", PartidLogo = "/images/default.png", ProcenteVot = 12.4 },
                        new { Candidat = "Elena Botezatu", Partid = "DA", PartidLogo = "/images/default.png", ProcenteVot = 8.3 }
                    }
                },
                new {
                    Id = "MD-BS",
                    Raion = "Bălți",
                    ProcenteVot = 72.8,
                    ProcenteFemei = 52.6,
                    ProcenteBarbati = 47.4,
                    Votanti = 81640,
                    Partid = "PSRM", // castigator
                    PartidLogo = "/images/default.png",
                    Candidat = "Sergiu Rusu",
                    Candidates = new[] {
                        new { Candidat = "Sergiu Rusu", Partid = "PSRM", PartidLogo = "/images/default.png", ProcenteVot = 35.2 },
                        new { Candidat = "Adrian Damaschin", Partid = "PAS", PartidLogo = "/images/default.png", ProcenteVot = 24.9 },
                        new { Candidat = "Tatiana Melnic", Partid = "MAN", PartidLogo = "/images/default.png", ProcenteVot = 15.4 },
                        new { Candidat = "Nicu Vrabie", Partid = "PPDA", PartidLogo = "/images/default.png", ProcenteVot = 13.1 },
                        new { Candidat = "Elena Ciobanu", Partid = "DA", PartidLogo = "/images/default.png", ProcenteVot = 11.4 }
                    }
                },
                new {
                    Id = "MD-CA",
                    Raion = "Cahul",
                    ProcenteVot = 65.7,
                    ProcenteFemei = 53.3,
                    ProcenteBarbati = 46.7,
                    Votanti = 51230,
                    Partid = "PPDA", // castigator
                    PartidLogo = "/images/default.png",
                    Candidat = "Victor Munteanu",
                    Candidates = new[] {
                        new { Candidat = "Victor Munteanu", Partid = "PPDA", PartidLogo = "/images/default.png", ProcenteVot = 33.8 },
                        new { Candidat = "Alina Țurcan", Partid = "PAS", PartidLogo = "/images/default.png", ProcenteVot = 25.6 },
                        new { Candidat = "Sergiu Bivol", Partid = "PSRM", PartidLogo = "/images/default.png", ProcenteVot = 17.9 },
                        new { Candidat = "Petru Rusu", Partid = "MAN", PartidLogo = "/images/default.png", ProcenteVot = 12.1 },
                        new { Candidat = "Doina Cazacu", Partid = "DA", PartidLogo = "/images/default.png", ProcenteVot = 10.6 }
                    }
                },
                new {
                    Id = "MD-CU",
                    Raion = "Chișinău",
                    ProcenteVot = 78.2,
                    ProcenteFemei = 57.9,
                    ProcenteBarbati = 42.1,
                    Votanti = 312450,
                    Partid = "PAS", // castigator
                    PartidLogo = "/images/default.png",
                    Candidat = "Andrei Lupu",
                    Candidates = new[] {
                        new { Candidat = "Andrei Lupu", Partid = "PAS", PartidLogo = "/images/default.png", ProcenteVot = 40.5 },
                        new { Candidat = "Ion Ceban", Partid = "MAN", PartidLogo = "/images/default.png", ProcenteVot = 21.7 },
                        new { Candidat = "Dumitru Colesnic", Partid = "PSRM", PartidLogo = "/images/default.png", ProcenteVot = 16.9 },
                        new { Candidat = "Valeria Muntean", Partid = "PPDA", PartidLogo = "/images/default.png", ProcenteVot = 12.4 },
                        new { Candidat = "Radu Beșleagă", Partid = "DA", PartidLogo = "/images/default.png", ProcenteVot = 8.5 }
                    }
                },
                new {
                    Id = "MD-OR",
                    Raion = "Orhei",
                    ProcenteVot = 70.5,
                    ProcenteFemei = 55.8,
                    ProcenteBarbati = 44.2,
                    Votanti = 60410,
                    Partid = "MAN", // castigator
                    PartidLogo = "/images/default.png",
                    Candidat = "Marin Țurcanu",
                    Candidates = new[] {
                        new { Candidat = "Marin Țurcanu", Partid = "MAN", PartidLogo = "/images/default.png", ProcenteVot = 36.6 },
                        new { Candidat = "Daniela Bivol", Partid = "PAS", PartidLogo = "/images/default.png", ProcenteVot = 22.8 },
                        new { Candidat = "Igor Țurcan", Partid = "PSRM", PartidLogo = "/images/default.png", ProcenteVot = 17.2 },
                        new { Candidat = "Tatiana Deliu", Partid = "PPDA", PartidLogo = "/images/default.png", ProcenteVot = 12.5 },
                        new { Candidat = "Andrei Rusnac", Partid = "DA", PartidLogo = "/images/default.png", ProcenteVot = 10.9 }
                    }
                },
                new {
                    Id = "MD-SO",
                    Raion = "Soroca",
                    ProcenteVot = 69.1,
                    ProcenteFemei = 56.4,
                    ProcenteBarbati = 43.6,
                    Votanti = 48870,
                    Partid = "DA", // castigator
                    PartidLogo = "/images/default.png",
                    Candidat = "Alexandru Bejan",
                    Candidates = new[] {
                        new { Candidat = "Alexandru Bejan", Partid = "DA", PartidLogo = "/images/default.png", ProcenteVot = 31.4 },
                        new { Candidat = "Irina Pînzari", Partid = "PAS", PartidLogo = "/images/default.png", ProcenteVot = 24.7 },
                        new { Candidat = "Victor Nagacevschi", Partid = "PSRM", PartidLogo = "/images/default.png", ProcenteVot = 18.6 },
                        new { Candidat = "Corneliu Tofan", Partid = "PPDA", PartidLogo = "/images/default.png", ProcenteVot = 14.1 },
                        new { Candidat = "Mihai Țugui", Partid = "MAN", PartidLogo = "/images/default.png", ProcenteVot = 11.2 }
                    }
                }
            };

            ViewBag.MapData = JsonSerializer.Serialize(mapData);
            return View();
        }
        public IActionResult ConsilieriLocali()
        {
            return View();
        }

        public IActionResult ConsilieriRaionali()
        {
            var mapData = new[]
            {
                new {
                    Id = "MD-AN",
                    Raion = "Anenii Noi",
                    ProcenteVot = 68.4,
                    ProcenteFemei = 54.1,
                    ProcenteBarbati = 45.9,
                    Votanti = 45210,
                    Partid = "PAS", // castigator
                    PartidLogo = "/images/default.png",
                    Candidat = "Ion Popescu",
                    Candidates = new[] {
                        new { Candidat = "Ion Popescu", Partid = "PAS", PartidLogo = "/images/default.png", ProcenteVot = 38.5 },
                        new { Candidat = "Vasile Corneanu", Partid = "PSRM", PartidLogo = "/images/default.png", ProcenteVot = 26.1 },
                        new { Candidat = "Maria Țîrdea", Partid = "PPDA", PartidLogo = "/images/default.png", ProcenteVot = 14.7 },
                        new { Candidat = "Mihai Grigoraș", Partid = "MAN", PartidLogo = "/images/default.png", ProcenteVot = 12.4 },
                        new { Candidat = "Elena Botezatu", Partid = "DA", PartidLogo = "/images/default.png", ProcenteVot = 8.3 }
                    }
                },
                new {
                    Id = "MD-BS",
                    Raion = "Bălți",
                    ProcenteVot = 72.8,
                    ProcenteFemei = 52.6,
                    ProcenteBarbati = 47.4,
                    Votanti = 81640,
                    Partid = "PSRM", // castigator
                    PartidLogo = "/images/default.png",
                    Candidat = "Sergiu Rusu",
                    Candidates = new[] {
                        new { Candidat = "Sergiu Rusu", Partid = "PSRM", PartidLogo = "/images/default.png", ProcenteVot = 35.2 },
                        new { Candidat = "Adrian Damaschin", Partid = "PAS", PartidLogo = "/images/default.png", ProcenteVot = 24.9 },
                        new { Candidat = "Tatiana Melnic", Partid = "MAN", PartidLogo = "/images/default.png", ProcenteVot = 15.4 },
                        new { Candidat = "Nicu Vrabie", Partid = "PPDA", PartidLogo = "/images/default.png", ProcenteVot = 13.1 },
                        new { Candidat = "Elena Ciobanu", Partid = "DA", PartidLogo = "/images/default.png", ProcenteVot = 11.4 }
                    }
                },
                new {
                    Id = "MD-CA",
                    Raion = "Cahul",
                    ProcenteVot = 65.7,
                    ProcenteFemei = 53.3,
                    ProcenteBarbati = 46.7,
                    Votanti = 51230,
                    Partid = "PPDA", // castigator
                    PartidLogo = "/images/default.png",
                    Candidat = "Victor Munteanu",
                    Candidates = new[] {
                        new { Candidat = "Victor Munteanu", Partid = "PPDA", PartidLogo = "/images/default.png", ProcenteVot = 33.8 },
                        new { Candidat = "Alina Țurcan", Partid = "PAS", PartidLogo = "/images/default.png", ProcenteVot = 25.6 },
                        new { Candidat = "Sergiu Bivol", Partid = "PSRM", PartidLogo = "/images/default.png", ProcenteVot = 17.9 },
                        new { Candidat = "Petru Rusu", Partid = "MAN", PartidLogo = "/images/default.png", ProcenteVot = 12.1 },
                        new { Candidat = "Doina Cazacu", Partid = "DA", PartidLogo = "/images/default.png", ProcenteVot = 10.6 }
                    }
                },
                new {
                    Id = "MD-CU",
                    Raion = "Chișinău",
                    ProcenteVot = 78.2,
                    ProcenteFemei = 57.9,
                    ProcenteBarbati = 42.1,
                    Votanti = 312450,
                    Partid = "PAS", // castigator
                    PartidLogo = "/images/default.png",
                    Candidat = "Andrei Lupu",
                    Candidates = new[] {
                        new { Candidat = "Andrei Lupu", Partid = "PAS", PartidLogo = "/images/default.png", ProcenteVot = 40.5 },
                        new { Candidat = "Ion Ceban", Partid = "MAN", PartidLogo = "/images/default.png", ProcenteVot = 21.7 },
                        new { Candidat = "Dumitru Colesnic", Partid = "PSRM", PartidLogo = "/images/default.png", ProcenteVot = 16.9 },
                        new { Candidat = "Valeria Muntean", Partid = "PPDA", PartidLogo = "/images/default.png", ProcenteVot = 12.4 },
                        new { Candidat = "Radu Beșleagă", Partid = "DA", PartidLogo = "/images/default.png", ProcenteVot = 8.5 }
                    }
                },
                new {
                    Id = "MD-OR",
                    Raion = "Orhei",
                    ProcenteVot = 70.5,
                    ProcenteFemei = 55.8,
                    ProcenteBarbati = 44.2,
                    Votanti = 60410,
                    Partid = "MAN", // castigator
                    PartidLogo = "/images/default.png",
                    Candidat = "Marin Țurcanu",
                    Candidates = new[] {
                        new { Candidat = "Marin Țurcanu", Partid = "MAN", PartidLogo = "/images/default.png", ProcenteVot = 36.6 },
                        new { Candidat = "Daniela Bivol", Partid = "PAS", PartidLogo = "/images/default.png", ProcenteVot = 22.8 },
                        new { Candidat = "Igor Țurcan", Partid = "PSRM", PartidLogo = "/images/default.png", ProcenteVot = 17.2 },
                        new { Candidat = "Tatiana Deliu", Partid = "PPDA", PartidLogo = "/images/default.png", ProcenteVot = 12.5 },
                        new { Candidat = "Andrei Rusnac", Partid = "DA", PartidLogo = "/images/default.png", ProcenteVot = 10.9 }
                    }
                },
                new {
                    Id = "MD-SO",
                    Raion = "Soroca",
                    ProcenteVot = 69.1,
                    ProcenteFemei = 56.4,
                    ProcenteBarbati = 43.6,
                    Votanti = 48870,
                    Partid = "DA", // castigator
                    PartidLogo = "/images/default.png",
                    Candidat = "Alexandru Bejan",
                    Candidates = new[] {
                        new { Candidat = "Alexandru Bejan", Partid = "DA", PartidLogo = "/images/default.png", ProcenteVot = 31.4 },
                        new { Candidat = "Irina Pînzari", Partid = "PAS", PartidLogo = "/images/default.png", ProcenteVot = 24.7 },
                        new { Candidat = "Victor Nagacevschi", Partid = "PSRM", PartidLogo = "/images/default.png", ProcenteVot = 18.6 },
                        new { Candidat = "Corneliu Tofan", Partid = "PPDA", PartidLogo = "/images/default.png", ProcenteVot = 14.1 },
                        new { Candidat = "Mihai Țugui", Partid = "MAN", PartidLogo = "/images/default.png", ProcenteVot = 11.2 }
                    }
                }
            };

            ViewBag.MapData = JsonSerializer.Serialize(mapData);
            return View();
        }

        public IActionResult PrimariMunicipali()
        {
            var mapData = new[]
            {
                new {
                    Id = "MD-AN",
                    Raion = "Anenii Noi",
                    ProcenteVot = 68.4,
                    ProcenteFemei = 54.1,
                    ProcenteBarbati = 45.9,
                    Votanti = 45210,
                    Partid = "PAS", // castigator
                    PartidLogo = "/images/default.png",
                    Candidat = "Ion Popescu",
                    Candidates = new[] {
                        new { Candidat = "Ion Popescu", Partid = "PAS", PartidLogo = "/images/default.png", ProcenteVot = 38.5 },
                        new { Candidat = "Vasile Corneanu", Partid = "PSRM", PartidLogo = "/images/default.png", ProcenteVot = 26.1 },
                        new { Candidat = "Maria Țîrdea", Partid = "PPDA", PartidLogo = "/images/default.png", ProcenteVot = 14.7 },
                        new { Candidat = "Mihai Grigoraș", Partid = "MAN", PartidLogo = "/images/default.png", ProcenteVot = 12.4 },
                        new { Candidat = "Elena Botezatu", Partid = "DA", PartidLogo = "/images/default.png", ProcenteVot = 8.3 }
                    }
                },
                new {
                    Id = "MD-BS",
                    Raion = "Bălți",
                    ProcenteVot = 72.8,
                    ProcenteFemei = 52.6,
                    ProcenteBarbati = 47.4,
                    Votanti = 81640,
                    Partid = "PSRM", // castigator
                    PartidLogo = "/images/default.png",
                    Candidat = "Sergiu Rusu",
                    Candidates = new[] {
                        new { Candidat = "Sergiu Rusu", Partid = "PSRM", PartidLogo = "/images/default.png", ProcenteVot = 35.2 },
                        new { Candidat = "Adrian Damaschin", Partid = "PAS", PartidLogo = "/images/default.png", ProcenteVot = 24.9 },
                        new { Candidat = "Tatiana Melnic", Partid = "MAN", PartidLogo = "/images/default.png", ProcenteVot = 15.4 },
                        new { Candidat = "Nicu Vrabie", Partid = "PPDA", PartidLogo = "/images/default.png", ProcenteVot = 13.1 },
                        new { Candidat = "Elena Ciobanu", Partid = "DA", PartidLogo = "/images/default.png", ProcenteVot = 11.4 }
                    }
                },
                new {
                    Id = "MD-CA",
                    Raion = "Cahul",
                    ProcenteVot = 65.7,
                    ProcenteFemei = 53.3,
                    ProcenteBarbati = 46.7,
                    Votanti = 51230,
                    Partid = "PPDA", // castigator
                    PartidLogo = "/images/default.png",
                    Candidat = "Victor Munteanu",
                    Candidates = new[] {
                        new { Candidat = "Victor Munteanu", Partid = "PPDA", PartidLogo = "/images/default.png", ProcenteVot = 33.8 },
                        new { Candidat = "Alina Țurcan", Partid = "PAS", PartidLogo = "/images/default.png", ProcenteVot = 25.6 },
                        new { Candidat = "Sergiu Bivol", Partid = "PSRM", PartidLogo = "/images/default.png", ProcenteVot = 17.9 },
                        new { Candidat = "Petru Rusu", Partid = "MAN", PartidLogo = "/images/default.png", ProcenteVot = 12.1 },
                        new { Candidat = "Doina Cazacu", Partid = "DA", PartidLogo = "/images/default.png", ProcenteVot = 10.6 }
                    }
                },
                new {
                    Id = "MD-CU",
                    Raion = "Chișinău",
                    ProcenteVot = 78.2,
                    ProcenteFemei = 57.9,
                    ProcenteBarbati = 42.1,
                    Votanti = 312450,
                    Partid = "PAS", // castigator
                    PartidLogo = "/images/default.png",
                    Candidat = "Andrei Lupu",
                    Candidates = new[] {
                        new { Candidat = "Andrei Lupu", Partid = "PAS", PartidLogo = "/images/default.png", ProcenteVot = 40.5 },
                        new { Candidat = "Ion Ceban", Partid = "MAN", PartidLogo = "/images/default.png", ProcenteVot = 21.7 },
                        new { Candidat = "Dumitru Colesnic", Partid = "PSRM", PartidLogo = "/images/default.png", ProcenteVot = 16.9 },
                        new { Candidat = "Valeria Muntean", Partid = "PPDA", PartidLogo = "/images/default.png", ProcenteVot = 12.4 },
                        new { Candidat = "Radu Beșleagă", Partid = "DA", PartidLogo = "/images/default.png", ProcenteVot = 8.5 }
                    }
                },
                new {
                    Id = "MD-OR",
                    Raion = "Orhei",
                    ProcenteVot = 70.5,
                    ProcenteFemei = 55.8,
                    ProcenteBarbati = 44.2,
                    Votanti = 60410,
                    Partid = "MAN", // castigator
                    PartidLogo = "/images/default.png",
                    Candidat = "Marin Țurcanu",
                    Candidates = new[] {
                        new { Candidat = "Marin Țurcanu", Partid = "MAN", PartidLogo = "/images/default.png", ProcenteVot = 36.6 },
                        new { Candidat = "Daniela Bivol", Partid = "PAS", PartidLogo = "/images/default.png", ProcenteVot = 22.8 },
                        new { Candidat = "Igor Țurcan", Partid = "PSRM", PartidLogo = "/images/default.png", ProcenteVot = 17.2 },
                        new { Candidat = "Tatiana Deliu", Partid = "PPDA", PartidLogo = "/images/default.png", ProcenteVot = 12.5 },
                        new { Candidat = "Andrei Rusnac", Partid = "DA", PartidLogo = "/images/default.png", ProcenteVot = 10.9 }
                    }
                },
                new {
                    Id = "MD-SO",
                    Raion = "Soroca",
                    ProcenteVot = 69.1,
                    ProcenteFemei = 56.4,
                    ProcenteBarbati = 43.6,
                    Votanti = 48870,
                    Partid = "DA", // castigator
                    PartidLogo = "/images/default.png",
                    Candidat = "Alexandru Bejan",
                    Candidates = new[] {
                        new { Candidat = "Alexandru Bejan", Partid = "DA", PartidLogo = "/images/default.png", ProcenteVot = 31.4 },
                        new { Candidat = "Irina Pînzari", Partid = "PAS", PartidLogo = "/images/default.png", ProcenteVot = 24.7 },
                        new { Candidat = "Victor Nagacevschi", Partid = "PSRM", PartidLogo = "/images/default.png", ProcenteVot = 18.6 },
                        new { Candidat = "Corneliu Tofan", Partid = "PPDA", PartidLogo = "/images/default.png", ProcenteVot = 14.1 },
                        new { Candidat = "Mihai Țugui", Partid = "MAN", PartidLogo = "/images/default.png", ProcenteVot = 11.2 }
                    }
                }
            };

            ViewBag.MapData = JsonSerializer.Serialize(mapData);

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

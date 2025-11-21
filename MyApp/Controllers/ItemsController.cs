using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using MyApp.Models;
using System;

namespace MyApp.Controllers
{
    public class ItemsController : Controller
    {

        private readonly UniversitateaContext _context;
        public ItemsController(UniversitateaContext context)
        {
            _context = context; 
        }

        public async Task<IActionResult> Index()
        {
            var items = await _context.Disciplines.ToListAsync(); 
            return View(items);
        }



        public IActionResult Overview()
        {
            var item = new Item() { Name = "Desk", Price = 9 };
            return View(item);
        }
    }
} 





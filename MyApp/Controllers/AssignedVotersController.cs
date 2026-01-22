using Microsoft.AspNetCore.Mvc;
using MyApp.Data;
using MyApp.Repositories;

namespace MyApp.Controllers
{
    public class AssignedVotersController : Controller
    {
        private readonly IAssignedVoterRepository _repository;
        private readonly DbService _dbService;

        public AssignedVotersController(IAssignedVoterRepository repository, DbService dbService)
        {
            _repository = repository;
            _dbService = dbService;
        }


        public async Task<IActionResult> Index()
        {
            try
            {
                var voters = await _repository.GetAllAsync();
                return View(voters);
            }
            catch (Exception ex)
            {
                // log ex here
                return Content($"Failed to load voters: {ex.Message}");
            }
        }

        public async Task<IActionResult> ByRegion(int regionId)
        {
            var voters = await _repository.GetByRegionAsync(regionId);
            return View("Index", voters);
        }

        public async Task<IActionResult> ByParentRegion(int regionId)
        {
            var voters = await _repository.GetByParentRegionAsync(regionId);
            return View("Index", voters);
        }

        public async Task<IActionResult> FromStoredProcedure()
        {
            var voters = await _dbService.GetVoterStatistics();
            return View("Index", voters);
        }
    }
}
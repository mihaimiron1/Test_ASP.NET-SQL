using Microsoft.AspNetCore.Mvc;
using MyApp.Repositories;

namespace MyApp.Controllers
{
    public class AssignedVotersController : Controller
    {
        private readonly IAssignedVoterRepository _repository;

        public AssignedVotersController(IAssignedVoterRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            var voters = await _repository.GetAllAsync();
            return View(voters);
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
    }
}
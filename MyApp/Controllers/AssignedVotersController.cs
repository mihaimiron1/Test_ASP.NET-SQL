using Microsoft.AspNetCore.Mvc;
using MyApp.Repositories;

namespace MyApp.Controllers
{
    public class AssignedVotersController : Controller
    {
        private readonly IAssignedVoterRepository _repository;
        private readonly ILogger<AssignedVotersController> _logger;

        public AssignedVotersController(
            IAssignedVoterRepository repository,
            ILogger<AssignedVotersController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("Loading all assigned voters");
                var voters = await _repository.GetAllAsync();
                return View(voters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load voters");
                return Content($"Failed to load voters: {ex.Message}");
            }
        }

        public async Task<IActionResult> ByRegion(int regionId)
        {
            try
            {
                _logger.LogInformation("Loading voters for RegionId={RegionId}", regionId);
                var voters = await _repository.GetByRegionAsync(regionId);
                return View("Index", voters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load voters for RegionId={RegionId}", regionId);
                return Content($"Failed to load voters: {ex.Message}");
            }
        }

        public async Task<IActionResult> ByParentRegion(int regionId)
        {
            try
            {
                _logger.LogInformation("Loading voters for ParentRegionId={ParentRegionId}", regionId);
                var voters = await _repository.GetByParentRegionAsync(regionId);
                return View("Index", voters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load voters for ParentRegionId={ParentRegionId}", regionId);
                return Content($"Failed to load voters: {ex.Message}");
            }
        }
    }
}
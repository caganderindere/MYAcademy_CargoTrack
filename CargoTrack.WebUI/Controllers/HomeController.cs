using CargoTrack.Business.Services.Branches;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Controllers
{
    public class HomeController(IBranchService _branchService) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Branches()
        {
            var branches = await _branchService.GetActiveAsync();
            return View(branches);
        }
    }
}

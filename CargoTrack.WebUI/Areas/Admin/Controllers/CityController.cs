using CargoTrack.Business.Services.Cities;
using CargoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Areas.Admin.Controllers
{
    [Area(Area.Admin)]
    [Authorize(Roles = Roles.Admin)]
    public class CityController(ICityService _cityService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View(await _cityService.GetAllAsync());
        }
    }
}

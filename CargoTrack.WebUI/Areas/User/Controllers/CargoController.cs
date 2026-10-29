using CargoTrack.Business.Services.Cargos;
using CargoTrack.DTO.DTOs.Cargos;
using CargoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Areas.User.Controllers
{
    [Area(Area.User)]
    [Authorize(Roles = Roles.User)]
    public class CargoController(ICargoService _cargoService) : Controller
    {
        public async Task<IActionResult> Index(CargoFilterDto filter, string? tab)
        {
            tab ??= "active";

            filter.OnlyActive = tab is "active" or "incoming" or "outgoing";
            filter.OnlyFinished = tab == "history";
            filter.Direction = tab is "incoming" or "outgoing" ? tab : null;

            ViewBag.Tab = tab;
            ViewBag.Filter = filter;
            return View(await _cargoService.GetPagedAsync(filter));
        }

        public async Task<IActionResult> Detail(Guid id)
        {
            return View(await _cargoService.GetDetailAsync(id));
        }
    }
}

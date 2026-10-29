using CargoTrack.Business.Services.Cargos;
using CargoTrack.DTO.DTOs.Cargos;
using CargoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Areas.Manager.Controllers
{
    [Area(Area.Manager)]
    [Authorize(Roles = Roles.AdminOrManager)]
    public class OperationController(ICargoService _cargoService) : Controller
    {
        public async Task<IActionResult> Index(MovementFilterDto filter)
        {
            filter.Date ??= DateTime.Today;
            ViewBag.Filter = filter;
            return View(await _cargoService.GetMovementsAsync(filter));
        }
    }
}

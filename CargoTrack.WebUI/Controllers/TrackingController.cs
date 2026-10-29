using CargoTrack.Business.Exceptions;
using CargoTrack.Business.Services.Cargos;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Controllers
{
    public class TrackingController(ICargoService _cargoService) : Controller
    {
        public IActionResult Index(string? code)
        {
            if (!string.IsNullOrWhiteSpace(code))
            {
                return RedirectToAction(nameof(Result), new { code });
            }

            return View();
        }

        public async Task<IActionResult> Result(string code)
        {
            try
            {
                var tracking = await _cargoService.GetPublicTrackingAsync(code);
                return View(tracking);
            }
            catch (BusinessException ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.Code = code;
                return View(nameof(Index));
            }
        }
    }
}

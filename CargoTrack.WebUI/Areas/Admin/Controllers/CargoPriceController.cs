using CargoTrack.Business.Services.CargoPrices;
using CargoTrack.DTO.DTOs.CargoPriceDtos;
using CargoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Areas.Admin.Controllers
{
    [Area(Area.Admin)]
    [Authorize(Roles = Roles.Admin)]
    public class CargoPriceController(ICargoPriceService _cargoPriceService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View(await _cargoPriceService.GetAllAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCargoPriceDto cargoPriceDto)
        {
            if (!ModelState.IsValid)
            {
                return View(cargoPriceDto);
            }

            await _cargoPriceService.CreateAsync(cargoPriceDto);
            TempData["Success"] = "Tarife eklendi.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(Guid id)
        {
            return View(await _cargoPriceService.GetByIdAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateCargoPriceDto cargoPriceDto)
        {
            if (!ModelState.IsValid)
            {
                return View(cargoPriceDto);
            }

            await _cargoPriceService.UpdateAsync(cargoPriceDto);
            TempData["Success"] = "Tarife güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _cargoPriceService.DeleteAsync(id);
            TempData["Success"] = "Tarife silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}

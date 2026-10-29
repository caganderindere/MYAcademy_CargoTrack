using CargoTrack.Business.Services.Abouts;
using CargoTrack.DTO.DTOs.AboutDtos;
using CargoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Areas.Admin.Controllers
{
    [Area(Area.Admin)]
    [Authorize(Roles = Roles.Admin)]
    public class AboutController(IAboutService _aboutService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View(await _aboutService.GetAllAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAboutDto aboutDto)
        {
            if (!ModelState.IsValid)
            {
                return View(aboutDto);
            }

            await _aboutService.CreateAsync(aboutDto);
            TempData["Success"] = "İçerik eklendi.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(Guid id)
        {
            return View(await _aboutService.GetByIdAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateAboutDto aboutDto)
        {
            if (!ModelState.IsValid)
            {
                return View(aboutDto);
            }

            await _aboutService.UpdateAsync(aboutDto);
            TempData["Success"] = "İçerik güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _aboutService.DeleteAsync(id);
            TempData["Success"] = "İçerik silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}

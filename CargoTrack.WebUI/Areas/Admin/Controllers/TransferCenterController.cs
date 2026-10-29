using CargoTrack.Business.Services.Cities;
using CargoTrack.Business.Services.TransferCenters;
using CargoTrack.DTO.DTOs.TransferCenterDtos;
using CargoTrack.WebUI.Extensions;
using CargoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Areas.Admin.Controllers
{
    [Area(Area.Admin)]
    [Authorize(Roles = Roles.Admin)]
    public class TransferCenterController(ITransferCenterService _transferCenterService,
                                          ICityService _cityService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View(await _transferCenterService.GetAllAsync());
        }

        public async Task<IActionResult> Create()
        {
            await LoadCitiesAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTransferCenterDto transferCenterDto)
        {
            if (!ModelState.IsValid)
            {
                await LoadCitiesAsync();
                return View(transferCenterDto);
            }

            await _transferCenterService.CreateAsync(transferCenterDto);
            TempData["Success"] = "Transfer merkezi oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(Guid id)
        {
            await LoadCitiesAsync();
            return View(await _transferCenterService.GetByIdAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateTransferCenterDto transferCenterDto)
        {
            if (!ModelState.IsValid)
            {
                await LoadCitiesAsync();
                return View(transferCenterDto);
            }

            await _transferCenterService.UpdateAsync(transferCenterDto);
            TempData["Success"] = "Transfer merkezi güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _transferCenterService.DeleteAsync(id);
            TempData["Success"] = "Transfer merkezi silindi.";
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadCitiesAsync()
        {
            var cities = await _cityService.GetAllAsync();
            ViewBag.Cities = cities.ToSelectList(x => x.Id, x => x.Name);
        }
    }
}

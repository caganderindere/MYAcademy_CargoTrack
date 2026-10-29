using CargoTrack.Business.Services.Branches;
using CargoTrack.Business.Services.Cities;
using CargoTrack.Business.Services.TransferCenters;
using CargoTrack.DTO.DTOs.BranchDtos;
using CargoTrack.WebUI.Extensions;
using CargoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Areas.Admin.Controllers
{
    [Area(Area.Admin)]
    [Authorize(Roles = Roles.Admin)]
    public class BranchController(IBranchService _branchService,
                                  ICityService _cityService,
                                  ITransferCenterService _transferCenterService) : Controller
    {
        public async Task<IActionResult> Index(BranchFilterDto filter)
        {
            await LoadCitiesAsync();
            ViewBag.Filter = filter;
            return View(await _branchService.GetPagedAsync(filter));
        }

        public async Task<IActionResult> Create()
        {
            await LoadSelectListsAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBranchDto branchDto)
        {
            if (!ModelState.IsValid)
            {
                await LoadSelectListsAsync();
                return View(branchDto);
            }

            await _branchService.CreateAsync(branchDto);
            TempData["Success"] = "Şube başarıyla oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(Guid id)
        {
            await LoadSelectListsAsync();
            return View(await _branchService.GetByIdAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateBranchDto branchDto)
        {
            if (!ModelState.IsValid)
            {
                await LoadSelectListsAsync();
                return View(branchDto);
            }

            await _branchService.UpdateAsync(branchDto);
            TempData["Success"] = "Şube bilgileri güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _branchService.DeleteAsync(id);
            TempData["Success"] = "Şube silindi.";
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadCitiesAsync()
        {
            var cities = await _cityService.GetAllAsync();
            ViewBag.Cities = cities.ToSelectList(x => x.Id, x => x.Name);
        }

        private async Task LoadSelectListsAsync()
        {
            await LoadCitiesAsync();
            var transferCenters = await _transferCenterService.GetAllAsync();
            ViewBag.TransferCenters = transferCenters.Where(x => x.IsActive).ToSelectList(x => x.Id, x => x.Name);
        }
    }
}

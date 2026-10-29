using CargoTrack.Business.Services.Branches;
using CargoTrack.Business.Services.Cities;
using CargoTrack.Business.Services.TransferCenters;
using CargoTrack.DTO.DTOs.BranchDtos;
using CargoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CargoTrack.WebUI.Areas.Admin.Controllers
{

    [Area(Area.Admin)]
    [Authorize(Roles = Roles.Admin)]
    public class BranchController(IBranchService _branchService,
                                  ICityService _cityService,
                                  ITransferCenterService _transferCenterService) : Controller
    {

        private async Task GetCitiesAsync()
        {
            var cities = await _cityService.GetAllAsync();

            var sortedCities = cities.OrderBy(x=>x.Name).ToList();

            ViewBag.cities = (from city in sortedCities
                              select new SelectListItem
                              {
                                  Text = city.Name,
                                  Value = city.Id.ToString()
                              }).ToList();
        }

        private async Task GetTransferCentersAsync()
        {
            var transferCenters = await _transferCenterService.GetAllAsync();

            ViewBag.transferCenters = (from transferCenter in transferCenters.OrderBy(x => x.Name)
                                       select new SelectListItem
                                       {
                                           Text = transferCenter.Name,
                                           Value = transferCenter.Id.ToString()
                                       }).ToList();
        }

        private async Task GetSelectListsAsync()
        {
            await GetCitiesAsync();
            await GetTransferCentersAsync();
        }

public async Task<IActionResult> Index()
        {
            var branches = await _branchService.GetAllAsync();
            return View(branches);
        }

        public async Task<IActionResult> Create()
        {
            await GetSelectListsAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBranchDto branchDto)
        {
            if (!ModelState.IsValid)
            {
                await GetSelectListsAsync();
                return View(branchDto);
            }

            await _branchService.CreateAsync(branchDto);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(Guid id)
        {
            await GetSelectListsAsync();
            var branch = await _branchService.GetByIdAsync(id);
            return View(branch);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateBranchDto branchDto)
        {
            if (!ModelState.IsValid)
            {
                await GetSelectListsAsync();
                return View(branchDto);
            }

            await _branchService.UpdateAsync(branchDto);
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Delete(Guid id)
        {
            await _branchService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

}
}

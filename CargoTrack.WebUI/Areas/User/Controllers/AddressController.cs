using CargoTrack.Business.Services.Addresses;
using CargoTrack.Business.Services.Cities;
using CargoTrack.DTO.DTOs.AddressDtos;
using CargoTrack.WebUI.Extensions;
using CargoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Areas.User.Controllers
{
    [Area(Area.User)]
    [Authorize(Roles = Roles.User)]
    public class AddressController(IAddressService _addressService,
                                   ICityService _cityService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View(await _addressService.GetMyAddressesAsync());
        }

        public async Task<IActionResult> Create()
        {
            await LoadCitiesAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAddressDto addressDto)
        {
            ModelState.Remove(nameof(CreateAddressDto.CustomerId));

            if (!ModelState.IsValid)
            {
                await LoadCitiesAsync();
                return View(addressDto);
            }

            await _addressService.CreateAsync(addressDto);
            TempData["Success"] = "Adresiniz kaydedildi.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(Guid id)
        {
            await LoadCitiesAsync();
            return View(await _addressService.GetByIdAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateAddressDto addressDto)
        {
            if (!ModelState.IsValid)
            {
                await LoadCitiesAsync();
                return View(addressDto);
            }

            await _addressService.UpdateAsync(addressDto);
            TempData["Success"] = "Adresiniz güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _addressService.DeleteAsync(id);
            TempData["Success"] = "Adres silindi.";
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadCitiesAsync()
        {
            var cities = await _cityService.GetAllAsync();
            ViewBag.Cities = cities.ToSelectList(x => x.Id, x => x.Name);
        }
    }
}

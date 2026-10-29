using CargoTrack.Business.Services.Addresses;
using CargoTrack.Business.Services.Cities;
using CargoTrack.Business.Services.Customers;
using CargoTrack.DTO.DTOs.AddressDtos;
using CargoTrack.DTO.DTOs.Common;
using CargoTrack.DTO.DTOs.CustomerDtos;
using CargoTrack.WebUI.Extensions;
using CargoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Areas.Manager.Controllers
{
    [Area(Area.Manager)]
    [Authorize(Roles = Roles.AdminOrManager)]
    public class CustomerController(ICustomerService _customerService,
                                    IAddressService _addressService,
                                    ICityService _cityService) : Controller
    {
        public async Task<IActionResult> Index(PageRequestDto request)
        {
            ViewBag.Search = request.Search;
            return View(await _customerService.GetPagedAsync(request));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCustomerDto customerDto)
        {
            if (!ModelState.IsValid)
            {
                return View(customerDto);
            }

            await _customerService.CreateAsync(customerDto);
            TempData["Success"] = "Müşteri kaydı oluşturuldu. Şimdi bir teslimat adresi ekleyebilirsiniz.";
            return RedirectToAction(nameof(Index), new { Search = customerDto.PhoneNumber });
        }

        public async Task<IActionResult> Update(Guid id)
        {
            return View(await _customerService.GetByIdAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateCustomerDto customerDto)
        {
            if (!ModelState.IsValid)
            {
                return View(customerDto);
            }

            await _customerService.UpdateAsync(customerDto);
            TempData["Success"] = "Müşteri bilgileri güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _customerService.DeleteAsync(id);
            TempData["Success"] = "Müşteri kaydı silindi.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Addresses(Guid id)
        {
            ViewBag.Customer = await _customerService.GetSummaryAsync(id);
            return View(await _addressService.GetByCustomerIdAsync(id));
        }

        public async Task<IActionResult> CreateAddress(Guid id)
        {
            await LoadCitiesAsync();
            ViewBag.Customer = await _customerService.GetSummaryAsync(id);
            return View(new CreateAddressDto { CustomerId = id });
        }

        [HttpPost]
        public async Task<IActionResult> CreateAddress(CreateAddressDto addressDto)
        {
            if (!ModelState.IsValid)
            {
                await LoadCitiesAsync();
                ViewBag.Customer = await _customerService.GetSummaryAsync(addressDto.CustomerId);
                return View(addressDto);
            }

            await _addressService.CreateAsync(addressDto);
            TempData["Success"] = "Adres eklendi.";
            return RedirectToAction(nameof(Addresses), new { id = addressDto.CustomerId });
        }

        public async Task<IActionResult> UpdateAddress(Guid id, Guid customerId)
        {
            await LoadCitiesAsync();
            ViewBag.CustomerId = customerId;
            return View(await _addressService.GetByIdAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAddress(UpdateAddressDto addressDto, Guid customerId)
        {
            if (!ModelState.IsValid)
            {
                await LoadCitiesAsync();
                ViewBag.CustomerId = customerId;
                return View(addressDto);
            }

            await _addressService.UpdateAsync(addressDto);
            TempData["Success"] = "Adres güncellendi.";
            return RedirectToAction(nameof(Addresses), new { id = customerId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAddress(Guid id, Guid customerId)
        {
            await _addressService.DeleteAsync(id);
            TempData["Success"] = "Adres silindi.";
            return RedirectToAction(nameof(Addresses), new { id = customerId });
        }

        private async Task LoadCitiesAsync()
        {
            var cities = await _cityService.GetAllAsync();
            ViewBag.Cities = cities.ToSelectList(x => x.Id, x => x.Name);
        }
    }
}

using CargoTrack.Business.Services.Branches;
using CargoTrack.Business.Services.Employees;
using CargoTrack.Business.Services.TransferCenters;
using CargoTrack.DTO.DTOs.EmployeeDtos;
using CargoTrack.WebUI.Extensions;
using CargoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Areas.Admin.Controllers
{
    [Area(Area.Admin)]
    [Authorize(Roles = Roles.Admin)]
    public class EmployeeController(IEmployeeService _employeeService,
                                    IBranchService _branchService,
                                    ITransferCenterService _transferCenterService) : Controller
    {
        public async Task<IActionResult> Index(EmployeeFilterDto filter)
        {
            await LoadSelectListsAsync();
            ViewBag.Filter = filter;
            return View(await _employeeService.GetPagedAsync(filter));
        }

        public async Task<IActionResult> Create()
        {
            await LoadSelectListsAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEmployeeDto employeeDto)
        {
            if (!ModelState.IsValid)
            {
                await LoadSelectListsAsync();
                return View(employeeDto);
            }

            await _employeeService.CreateAsync(employeeDto);
            TempData["Success"] = "Personel kaydı oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(Guid id)
        {
            await LoadSelectListsAsync();
            return View(await _employeeService.GetByIdAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateEmployeeDto employeeDto)
        {
            if (!ModelState.IsValid)
            {
                await LoadSelectListsAsync();
                return View(employeeDto);
            }

            await _employeeService.UpdateAsync(employeeDto);
            TempData["Success"] = "Personel bilgileri güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _employeeService.DeleteAsync(id);
            TempData["Success"] = "Personel kaydı silindi ve hesabı kilitlendi.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> CreateAccount(Guid id)
        {
            ViewBag.Employee = await _employeeService.GetSummaryAsync(id);
            return View(new CreateEmployeeAccountDto { EmployeeId = id });
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount(CreateEmployeeAccountDto accountDto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Employee = await _employeeService.GetSummaryAsync(accountDto.EmployeeId);
                return View(accountDto);
            }

            await _employeeService.CreateAccountAsync(accountDto);
            TempData["Success"] = "Personel hesabı oluşturuldu ve Manager rolü atandı.";
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadSelectListsAsync()
        {
            var branches = await _branchService.GetActiveAsync();
            var transferCenters = await _transferCenterService.GetAllAsync();

            ViewBag.Branches = branches.ToSelectList(x => x.Id, x => $"{x.Name} ({x.CityName})");
            ViewBag.TransferCenters = transferCenters.Where(x => x.IsActive).ToSelectList(x => x.Id, x => x.Name);
        }
    }
}

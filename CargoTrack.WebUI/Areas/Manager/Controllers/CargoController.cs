using CargoTrack.Business.Services.Addresses;
using CargoTrack.Business.Services.Branches;
using CargoTrack.Business.Services.Cargos;
using CargoTrack.Business.Services.Customers;
using CargoTrack.Business.Services.TransferCenters;
using CargoTrack.DTO.DTOs.Cargos;
using CargoTrack.Entity.Entities.Enums;
using CargoTrack.WebUI.Extensions;
using CargoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Areas.Manager.Controllers
{
    [Area(Area.Manager)]
    [Authorize(Roles = Roles.AdminOrManager)]
    public class CargoController(ICargoService _cargoService,
                                 IBranchService _branchService,
                                 ICustomerService _customerService,
                                 IAddressService _addressService,
                                 ITransferCenterService _transferCenterService) : Controller
    {
        public async Task<IActionResult> Index(CargoFilterDto filter)
        {
            return await ListAsync(filter, "Kargolar", "Biriminize bağlı tüm gönderileri arayın, filtreleyin ve yönetin.");
        }

        public async Task<IActionResult> Distribution(CargoFilterDto filter)
        {
            filter.Status = CargoStatus.OutForDelivery;
            filter.Direction ??= "incoming";
            return await ListAsync(filter, "Dağıtım Listesi", "Dağıtıma çıkmış gönderiler. Teslimat kodunu doğrulayarak teslim edin.");
        }

        public async Task<IActionResult> Problems(CargoFilterDto filter)
        {
            filter.OnlyProblematic = true;
            return await ListAsync(filter, "Sorunlu Kargolar", "Teslim edilemeyen ve iade sürecindeki gönderiler.");
        }

        public async Task<IActionResult> Detail(Guid id)
        {
            var detail = await _cargoService.GetDetailAsync(id);
            var transferCenters = await _transferCenterService.GetAllAsync();

            ViewBag.TransferCenters = transferCenters
                .Where(x => x.IsActive)
                .ToSelectList(x => x.Id, x => $"{x.Name} ({x.CityName})", detail.OriginBranchTransferCenterId ?? detail.DestinationBranchTransferCenterId);

            return View(detail);
        }

        public async Task<IActionResult> Create()
        {
            await LoadCreateContextAsync();
            return View(new CreateCargoDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCargoDto cargoDto)
        {
            if (!ModelState.IsValid)
            {
                await LoadCreateContextAsync();
                return View(cargoDto);
            }

            var trackCode = await _cargoService.CreateAsync(cargoDto);
            TempData["Success"] = $"Gönderi oluşturuldu. Takip numarası: {trackCode}";
            return RedirectToAction(nameof(Index), new { Search = trackCode });
        }

        [HttpPost]
        public async Task<IActionResult> Quote(CreateCargoDto cargoDto)
        {
            return Json(await _cargoService.QuoteAsync(cargoDto));
        }

        public async Task<IActionResult> SearchCustomers(string term)
        {
            return Json(await _customerService.SearchAsync(term));
        }

        public async Task<IActionResult> CustomerAddresses(Guid customerId)
        {
            return Json(await _addressService.GetByCustomerIdAsync(customerId));
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(ChangeCargoStatusDto statusDto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = string.Join(" ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                return RedirectToAction(nameof(Detail), new { id = statusDto.CargoId });
            }

            await _cargoService.ChangeStatusAsync(statusDto);
            TempData["Success"] = $"Kargo durumu '{statusDto.NewStatus.GetDisplayName()}' olarak güncellendi.";
            return RedirectToAction(nameof(Detail), new { id = statusDto.CargoId });
        }

        [HttpPost]
        public async Task<IActionResult> Deliver(DeliverCargoDto deliverDto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = string.Join(" ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                return RedirectToAction(nameof(Detail), new { id = deliverDto.CargoId });
            }

            await _cargoService.DeliverAsync(deliverDto);
            TempData["Success"] = "Teslimat kodu doğrulandı, gönderi teslim edildi.";
            return RedirectToAction(nameof(Detail), new { id = deliverDto.CargoId });
        }

        [HttpPost]
        public async Task<IActionResult> ReportFailed(FailedDeliveryDto failedDto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = string.Join(" ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                return RedirectToAction(nameof(Detail), new { id = failedDto.CargoId });
            }

            await _cargoService.ReportFailedDeliveryAsync(failedDto);
            TempData["Success"] = "Teslimat sorunu kaydedildi.";
            return RedirectToAction(nameof(Detail), new { id = failedDto.CargoId });
        }

        private async Task<IActionResult> ListAsync(CargoFilterDto filter, string title, string subtitle)
        {
            ViewBag.Filter = filter;
            ViewBag.Title = title;
            ViewBag.Subtitle = subtitle;
            ViewBag.Action = ControllerContext.ActionDescriptor.ActionName;
            return View("Index", await _cargoService.GetPagedAsync(filter));
        }

        private async Task LoadCreateContextAsync()
        {
            var context = await _cargoService.GetCreateContextAsync();
            var branches = await _branchService.GetActiveAsync();

            ViewBag.Context = context;
            ViewBag.Branches = branches.ToSelectList(x => x.Id, x => $"{x.Name} ({x.CityName})");
        }
    }
}

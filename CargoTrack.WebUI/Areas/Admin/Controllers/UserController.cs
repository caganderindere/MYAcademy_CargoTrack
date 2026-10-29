using CargoTrack.Business.Services.Users;
using CargoTrack.DTO.DTOs.Common;
using CargoTrack.DTO.DTOs.UserDtos;
using CargoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Areas.Admin.Controllers
{
    [Area(Area.Admin)]
    [Authorize(Roles = Consts.Roles.Admin)]
    public class UserController(IUserService _userService) : Controller
    {
        public async Task<IActionResult> Index(PageRequestDto request)
        {
            ViewBag.Search = request.Search;
            return View(await _userService.GetPagedAsync(request));
        }

        public async Task<IActionResult> Roles(Guid id)
        {
            return View(await _userService.GetRolesAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> Roles(UserRolesDto userRolesDto)
        {
            await _userService.AssignRolesAsync(userRolesDto);
            TempData["Success"] = "Kullanıcı rolleri güncellendi.";
            return RedirectToAction(nameof(Index));
        }
    }
}

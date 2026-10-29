using CargoTrack.Business.Exceptions;
using CargoTrack.Business.Services.Auth;
using CargoTrack.DTO.DTOs.UserDtos;
using CargoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Controllers
{
    public class LoginController(IAuthService _authService) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginUserDto loginUserDto)
        {
            if (!ModelState.IsValid)
            {
                return View(loginUserDto);
            }

            try
            {
                var userRoles = await _authService.LoginAsync(loginUserDto);
                return RedirectByRole(userRoles);
            }
            catch (BusinessException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(loginUserDto);
            }
        }

        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return RedirectToAction(nameof(Index));
        }

        private IActionResult RedirectByRole(IList<string> userRoles)
        {
            if (userRoles.Contains(Roles.Admin))
            {
                return Redirect("/Admin/Branch/Index");
            }

            if (userRoles.Contains(Roles.Manager))
            {
                return Redirect("/Manager/Dashboard/Index");
            }

            if (userRoles.Contains(Roles.User))
            {
                return Redirect("/User/Dashboard/Index");
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
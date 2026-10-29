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
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToPanel();
            }

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
            TempData["Success"] = "Oturumunuz güvenli şekilde kapatıldı.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Panel()
        {
            return RedirectToPanel();
        }

        private IActionResult RedirectToPanel()
        {
            var roles = new List<string>();

            if (User.IsInRole(Roles.Admin)) roles.Add(Roles.Admin);
            if (User.IsInRole(Roles.Manager)) roles.Add(Roles.Manager);
            if (User.IsInRole(Roles.User)) roles.Add(Roles.User);

            return RedirectByRole(roles);
        }

        private IActionResult RedirectByRole(IList<string> userRoles)
        {
            if (userRoles.Contains(Roles.Admin))
            {
                return Redirect("/Admin/Dashboard");
            }

            if (userRoles.Contains(Roles.Manager))
            {
                return Redirect("/Manager/Dashboard");
            }

            if (userRoles.Contains(Roles.User))
            {
                return Redirect("/User/Dashboard");
            }

            return RedirectToAction("Index", "Home");
        }
    }
}

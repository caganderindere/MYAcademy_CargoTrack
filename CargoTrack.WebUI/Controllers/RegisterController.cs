using CargoTrack.Business.Exceptions;
using CargoTrack.Business.Services.Auth;
using CargoTrack.DTO.DTOs.UserDtos;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Controllers
{
    public class RegisterController(IAuthService _authService) : Controller
    {
        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Signup(RegisterUserDto registerUserDto)
        {
            if (!ModelState.IsValid)
            {
                return View(registerUserDto);
            }

            try
            {
                await _authService.RegisterAsync(registerUserDto);
            }
            catch (BusinessException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(registerUserDto);
            }

            return RedirectToAction("Index", "Login");
        }
    }
}
using CargoTrack.WebUI.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CargoTrack.WebUI.Controllers
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ErrorController(ILogger<ErrorController> _logger) : Controller
    {
        [Route("Error")]
        public IActionResult Index()
        {
            var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (feature is not null)
            {
                _logger.LogError(feature.Error, "Beklenmeyen hata: {Path}", feature.Path);
            }

            Response.StatusCode = StatusCodes.Status500InternalServerError;

            return View("Page", new ErrorPageViewModel
            {
                StatusCode = 500,
                Title = "Beklenmeyen bir hata oluştu",
                Message = "İsteğiniz işlenirken bir sorun yaşandı. Ekibimiz bilgilendirildi, lütfen birkaç dakika sonra tekrar deneyin.",
                Icon = "bi-cone-striped",
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }

        [Route("Error/Status/{code:int}")]
        public IActionResult Status(int code)
        {
            Response.StatusCode = code;

            var model = code switch
            {
                404 => new ErrorPageViewModel { StatusCode = 404, Title = "Sayfa bulunamadı", Message = "Aradığınız sayfa taşınmış ya da hiç var olmamış olabilir.", Icon = "bi-signpost-split" },
                403 => new ErrorPageViewModel { StatusCode = 403, Title = "Erişim engellendi", Message = "Bu sayfayı görüntüleme yetkiniz bulunmuyor.", Icon = "bi-shield-lock" },
                _ => new ErrorPageViewModel { StatusCode = code, Title = "Bir şeyler ters gitti", Message = "İsteğiniz tamamlanamadı.", Icon = "bi-exclamation-diamond" }
            };

            return View("Page", model);
        }

        [Route("Error/AccessDenied")]
        public IActionResult AccessDenied()
        {
            Response.StatusCode = StatusCodes.Status403Forbidden;

            return View("Page", new ErrorPageViewModel
            {
                StatusCode = 403,
                Title = "Yetkisiz erişim",
                Message = "Bu alan rolünüz için kapalı. Doğru hesapla giriş yaptığınızdan emin olun.",
                Icon = "bi-shield-lock"
            });
        }
    }
}

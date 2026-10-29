using CargoTrack.Business.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace CargoTrack.WebUI.Filters
{
    public class BusinessExceptionFilter(ITempDataDictionaryFactory _tempDataFactory,
                                         ILogger<BusinessExceptionFilter> _logger) : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is not BusinessException exception)
            {
                return;
            }

            var request = context.HttpContext.Request;
            _logger.LogWarning("İş kuralı ihlali: {Message} ({Path})", exception.Message, request.Path);

            if (request.Headers.XRequestedWith == "XMLHttpRequest")
            {
                context.Result = new JsonResult(new { message = exception.Message }) { StatusCode = StatusCodes.Status400BadRequest };
                context.ExceptionHandled = true;
                return;
            }

            var tempData = _tempDataFactory.GetTempData(context.HttpContext);
            tempData["Error"] = exception.Message;

            var current = request.Path + request.QueryString;
            var referer = request.Headers.Referer.ToString();
            var refererPath = Uri.TryCreate(referer, UriKind.Absolute, out var uri) && uri.Host == request.Host.Host
                ? uri.PathAndQuery
                : null;

            context.Result = new RedirectResult(refererPath is not null && (HttpMethods.IsPost(request.Method) || refererPath != current)
                ? refererPath
                : "/");
            context.ExceptionHandled = true;
        }
    }
}

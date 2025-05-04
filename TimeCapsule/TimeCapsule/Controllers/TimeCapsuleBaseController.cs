using Microsoft.AspNetCore.Mvc;
using System.Net;
using TimeCapsule.Services.Responses;
using TimeCapsule.Services.Results;

namespace TimeCapsule.Controllers
{
    public abstract class TimeCapsuleBaseController : Controller
    {
        protected IActionResult ErrorPage(string message = "", HttpStatusCode? statusCode = null)
        {
            return Redirect($"/Error?message={message}&statusCode={statusCode}");
        }
        protected IActionResult ErrorPage<T>(ServiceResult<T> serviceResult)
        {
            return Redirect($"/Error?message={serviceResult.Error.Description}&statusCode={serviceResult.StatusCode}");
        }
        protected IActionResult ErrorPage(ServiceResult serviceResult)
        {
            return Redirect($"/Error?message={serviceResult.Error.Description}&statusCode={serviceResult.StatusCode}");
        }

        protected IActionResult HandleStatusCodeServiceResult<T>(ServiceResult<T> serviceResult, int? customStatusCode = null)
        {
            if (serviceResult.IsSuccess)
            {
                if (typeof(T) == typeof(FileMemoryStreamResponse))
                {
                    var fileData = serviceResult.Data as FileMemoryStreamResponse;
                    return File(fileData.MemoryStream, fileData.ContentType, fileData.FileDownloadName);
                }
                else if (typeof(T) == typeof(FileByteArrayResponse))
                {
                    var fileData = serviceResult.Data as FileByteArrayResponse;
                    return File(fileData.Bytes, fileData.ContentType, fileData.FileDownloadName);
                }
                return customStatusCode.HasValue ? StatusCode(customStatusCode.Value, serviceResult.Data) : Ok(serviceResult.Data);
            }
            //else if (serviceResult.StatusCode.HasValue)
            //    return StatusCode((int)serviceResult.StatusCode);

            var errors = new Dictionary<string, string[]>() { { "Model", serviceResult.Error.Errors.ToArray() } };

            var vpd = new ValidationProblemDetails(errors);
            vpd.Detail = serviceResult.Error.Description;
            vpd.Instance = $"{HttpContext.Request.Method} {HttpContext.Request.Path}";
            vpd.Type = $"Error {serviceResult.Error.Code}";
            vpd.Title = "Bad request";
            vpd.Status = (int)HttpStatusCode.BadRequest;
            vpd.Extensions.Add("traceId", HttpContext.TraceIdentifier);

            return ValidationProblem(vpd);
        }

        protected IActionResult HandleServiceResult(ServiceResult serviceResult, int? customStatusCode = null)
        {
            if (serviceResult.IsSuccess)
                return customStatusCode.HasValue ? StatusCode(customStatusCode.Value) : Ok();
            //else if (serviceResult.StatusCode.HasValue)
            //    return StatusCode((int)serviceResult.StatusCode);

            var errors = new Dictionary<string, string[]>() { { "Model", serviceResult.Error.Errors.ToArray() } };

            var vpd = new ValidationProblemDetails(errors);
            vpd.Detail = serviceResult.Error.Description;
            vpd.Instance = $"{HttpContext.Request.Method} {HttpContext.Request.Path}";
            vpd.Type = $"Error {serviceResult.Error.Code}";
            vpd.Title = "Bad request";
            vpd.Status = (int)HttpStatusCode.BadRequest;
            vpd.Extensions.Add("traceId", HttpContext.TraceIdentifier);

            return ValidationProblem(vpd);
        }

        protected IActionResult HandleFormResult<T>(
            ServiceResult result,
            T model,
            string successMessage,
            string errorPrefix = "Nieprawidłowe dane: ",
            string actionName = "GetForms")
            where T : class
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                TempData["ErrorMessage"] = errorPrefix + errors;
                var idProperty = typeof(T).GetProperty("Id");
                var id = idProperty?.GetValue(model)?.ToString() ?? "unknown";
                TempData["ErrorMessageId"] = $"error_{typeof(T).Name.ToLower()}_{id}_{DateTime.UtcNow.Ticks}";
                return RedirectToAction(actionName);
            }

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = successMessage;
                var idProperty = typeof(T).GetProperty("Id");
                var id = idProperty?.GetValue(model)?.ToString() ?? "unknown";
                TempData["SuccessMessageId"] = $"{typeof(T).Name.ToLower()}_update_{id}_{DateTime.UtcNow.Ticks}";
            }
            else
            {
                TempData["ErrorMessage"] = result.Error?.Description ?? "Wystąpił błąd podczas przetwarzania.";
                var idProperty = typeof(T).GetProperty("Id");
                var id = idProperty?.GetValue(model)?.ToString() ?? "unknown";
                TempData["ErrorMessageId"] = $"error_{typeof(T).Name.ToLower()}_{id}_{DateTime.UtcNow.Ticks}";
            }

            return RedirectToAction(actionName);
        }
    }
}

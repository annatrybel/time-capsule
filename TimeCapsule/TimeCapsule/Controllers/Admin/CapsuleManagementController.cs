using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TimeCapsule.Interfaces;
using TimeCapsule.Models;
using TimeCapsule.Models.Dto;
using TimeCapsule.Services;
using TimeCapsule.Services.Results;

namespace TimeCapsule.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("AdminPanel/Capsules")]
    public class CapsuleManagementController : TimeCapsuleBaseController
    {
        private readonly ICapsuleManagementService _capsuleManagementService;
        private readonly UserManager<IdentityUser> _userManager;

        public CapsuleManagementController(ICapsuleManagementService capsuleManagementService, UserManager<IdentityUser> userManager)
        {
            _capsuleManagementService = capsuleManagementService;
            _userManager = userManager;
        }


        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult CapsuleManagementPanel()
        {
            return View("~/Views/AdminPanel/Capsules/CapsuleManagementView.cshtml");
        }


        [HttpPost("GetCapsules")]
        public async Task<IActionResult> GetCapsules([FromForm] DataTableRequest request)
        {
            var serviceResponse = await _capsuleManagementService.GetCapsules(request);
            return HandleStatusCodeServiceResult(serviceResponse);
        }

        [HttpGet("GetCapsuleOpeningDate/{capsuleId}")]
        public async Task<IActionResult> GetCapsuleOpeningDate(int capsuleId)
        {
            if (capsuleId <= 0)
            {
                return BadRequest(ServiceResult.Failure("Nieprawidłowy identyfikator kapsuły"));
            }

            var serviceResponse = await _capsuleManagementService.GetCapsuleOpeningDate(capsuleId);
            return HandleStatusCodeServiceResult(serviceResponse);
        }

        [HttpPost("UpdateOpeningDate")]
        public async Task<IActionResult> UpdateOpeningDate(UpdateCapsuleOpeningDateDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                       .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                       .ToList();
                return BadRequest(ServiceResult.Failure("Invalid data:\n" + string.Join("\n", errors)));
            }
            var serviceResponse = await _capsuleManagementService.UpdateCapsuleOpeningDate(model);
            if (serviceResponse.IsSuccess)
            {
                TempData["SuccessMessage"] = $"Opening date updated successfully";
                TempData["SuccessMessageId"] = $"openingDate_update_{model.CapsuleId}_{DateTime.UtcNow.Ticks}";
                return HandleServiceResult(serviceResponse);
            }
            else
            {
                return BadRequest(ServiceResult.Failure(serviceResponse.Error.Description));
            }
        }

        [HttpGet("GetCapsuleRecipients/{capsuleId}")]
        public async Task<IActionResult> GetCapsuleRecipients(int capsuleId)
        {
            if (capsuleId <= 0)
            {
                return BadRequest(ServiceResult.Failure("Nieprawidłowy identyfikator kapsuły"));
            }

            var serviceResponse = await _capsuleManagementService.GetCapsuleRecipients(capsuleId);
            return HandleStatusCodeServiceResult(serviceResponse);
        }

        [HttpPost("UpdateRecipients")]
        public async Task<IActionResult> UpdateRecipients(UpdateCapsuleRecipientsDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                       .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                       .ToList();
                return BadRequest(ServiceResult.Failure("Invalid data:\n" + string.Join("\n", errors)));
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Nie można zidentyfikować użytkownika. Prosimy zalogować się ponownie.";
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Nie znaleziono konta użytkownika. Prosimy zalogować się ponownie.";
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var serviceResponse = await _capsuleManagementService.UpdateCapsuleRecipients(model,user);

            if (serviceResponse.IsSuccess)
            {
                TempData["SuccessMessage"] = $"List of recipients updated successfully";
                TempData["SuccessMessageId"] = $"recipients_update_{model.CapsuleId}_{DateTime.UtcNow.Ticks}";
                return HandleServiceResult(serviceResponse);
            }
            else
            {
                return BadRequest(ServiceResult.Failure(serviceResponse.Error.Description));
            }
        }
    }
}







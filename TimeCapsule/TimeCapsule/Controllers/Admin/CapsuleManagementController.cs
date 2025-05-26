using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TimeCapsule.Interfaces;
using TimeCapsule.Models;
using TimeCapsule.Services;

namespace TimeCapsule.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("AdminPanel/Capsules")]
    public class CapsuleManagementController : TimeCapsuleBaseController
    {
        private readonly ICapsuleManagementService _capsuleManagementService;

        public CapsuleManagementController(ICapsuleManagementService capsuleManagementService)
        {
            _capsuleManagementService = capsuleManagementService;
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
    }
}




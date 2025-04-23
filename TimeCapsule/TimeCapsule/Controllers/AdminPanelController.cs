using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TimeCapsule.Services;
using TimeCapsule.Models;
using TimeCapsule.Models.Dto;
using TimeCapsule.Services.Results;

namespace TimeCapsule.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("AdminPanel")]
    public class AdminPanelController : TimeCapsuleBaseController
    {
        private readonly AdminPanelService _adminPanelService;

        public AdminPanelController(AdminPanelService adminPanelService)
        {
            _adminPanelService = adminPanelService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("Users")]
        public IActionResult GetUsers()
        {
            return View("~/Views/AdminPanel/UsersManagementView.cshtml");
        }

        [HttpPost("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers([FromForm] DataTableRequest request)
        {
            var serviceResponse = await _adminPanelService.GetUsers(request);
            return HandleStatusCodeServiceResult(serviceResponse);
        }

        
        [HttpPost("LockUser/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> LockUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(ServiceResult.Failure("Invalid parameter: User ID cannot be empty"));
            }

            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == currentUserId)
            {
                return BadRequest(ServiceResult.Failure("Cannot lock your own account"));
            }

            var result = await _adminPanelService.LockUser(userId);
            return HandleServiceResult(result);
        }

        [HttpPost("UnlockUser/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UnlockUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(ServiceResult.Failure("Invalid parameter: User ID cannot be empty"));
            }

            var result = await _adminPanelService.UnlockUser(userId);
            return HandleServiceResult(result);
        }


        [Route("Forms")]
        public async Task<IActionResult> GetForms()
        {
            var sectionsResult = await _adminPanelService.GetFormSectionsWithQuestions();

            if (!sectionsResult.IsSuccess)
            {
                return View("~/Views/AdminPanel/FormsManagementView.cshtml", new List<CapsuleSectionDto>());
            }

            return View("~/Views/AdminPanel/FormsManagementView.cshtml", sectionsResult.Data);
        }

        [HttpPost("AddSection")]
        public async Task<IActionResult> AddSection([FromForm] CreateSectionDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult.Failure("Invalid section data"));
            }

            var result = await _adminPanelService.AddSection(model);
            return RedirectToAction("Forms");
        }

        [HttpPost("AddQuestion")]
        public async Task<IActionResult> AddQuestion([FromForm] CreateQuestionDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult.Failure("Invalid q data"));
            }

            var result = await _adminPanelService.AddQuestion(model);
            return RedirectToAction("Forms");
        }

        [Route("Capsules")]
        public IActionResult GetCapsules()
        {
            return View();
        }

    }
}

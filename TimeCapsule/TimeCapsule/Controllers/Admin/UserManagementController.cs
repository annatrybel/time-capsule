using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TimeCapsule.Services;
using TimeCapsule.Models;
using TimeCapsule.Models.Dto;
using TimeCapsule.Services.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace TimeCapsule.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("AdminPanel/Users")]
    public class UserManagementController : TimeCapsuleBaseController
    {
        private readonly AdminPanelService _adminPanelService;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserManagementController(AdminPanelService adminPanelService, RoleManager<IdentityRole> roleManager)
        {
            _adminPanelService = adminPanelService;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> GetUsers()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var viewModel = new UserDto
            {
                AvailableRoles = roles
            };
            return View("~/Views/AdminPanel/Users/UsersManagementView.cshtml", viewModel);
        }

        [HttpPost("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers([FromForm] DataTableRequest request)
        {
            var serviceResponse = await _adminPanelService.GetUsers(request);
            return HandleStatusCodeServiceResult(serviceResponse);
        }

        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser(UserDto user)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                       .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                       .ToList();
                return BadRequest(ServiceResult.Failure("Invalid data:\n" + string.Join("\n", errors)));
            }
            var result = await _adminPanelService.CreateUser(user);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = $"User {user.UserName} created successfully";
                TempData["SuccessMessageId"] = $"user_create_{user.UserId}_{DateTime.UtcNow.Ticks}";
                return HandleStatusCodeServiceResult(result);
            }
            else
            {
                return BadRequest(ServiceResult.Failure(result.Error.Description));
            }
        }

        [HttpPost("UpdateUser")]
        public async Task<IActionResult> UpdateUser(UserDto user)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                       .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                       .ToList();
                return BadRequest(ServiceResult.Failure("Invalid data", string.Join("\n", errors)));
            }
            var result = await _adminPanelService.UpdateUser(user);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = $"User {user.UserName} updated successfully";
                TempData["SuccessMessageId"] = $"user_update_{user.UserId}_{DateTime.UtcNow.Ticks}";
                return HandleServiceResult(result);
            }
            else
            {
                return BadRequest(ServiceResult.Failure(result.Error.Description));
            }
        }

        [HttpPost("LockUser/{userId}")]
        public async Task<IActionResult> LockUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Nieprawidłowy identyfikator użytkownika";
                return RedirectToAction("GetUsers");
            }

            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == currentUserId)
            {
                TempData["ErrorMessage"] = "Nie można zablokować własnego konta";
                return RedirectToAction("GetUsers");
            }

            var result = await _adminPanelService.LockUser(userId);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Konto użytkownika zostało zablokowane";
            }
            else
            {
                TempData["ErrorMessage"] = $"Wystąpił błąd podczas blokowania konta: {result.Error?.Description}";
            }

            return RedirectToAction("GetUsers");
        }

        [HttpPost("UnlockUser/{userId}")]
        public async Task<IActionResult> UnlockUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Nieprawidłowy identyfikator użytkownika";
                return RedirectToAction("GetUsers");
            }

            var result = await _adminPanelService.UnlockUser(userId);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Konto użytkownika zostało odblokowane";
            }
            else
            {
                TempData["ErrorMessage"] = $"Wystąpił błąd podczas odblokowywania konta: {result.Error?.Description}";
            }

            return RedirectToAction("GetUsers");
        }

        [HttpGet("GetUserById")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(ServiceResult.Failure("Invalid parameter: User ID cannot be empty"));
            }

            var result = await _adminPanelService.GetUserById(userId);
            return HandleStatusCodeServiceResult(result);
        }
    }
}

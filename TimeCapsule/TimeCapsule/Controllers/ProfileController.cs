using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TimeCapsule.Services;

namespace TimeCapsule.Controllers
{
    [Authorize]
    public class ProfileController : TimeCapsuleBaseController
    {
        private readonly ProfileService _profileService;

        public ProfileController(ILogger<HomeController> logger, ProfileService profileService)
        {
            _profileService = profileService;
        }
        public async Task<IActionResult> MyCapsules()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Nie można zidentyfikować użytkownika. Prosimy zalogować się ponownie.";
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }
            var result = await _profileService.GetMyCapsules(userId);

            return View("~/Views/Home/MyCapsules.cshtml", result);
        }
    }
}



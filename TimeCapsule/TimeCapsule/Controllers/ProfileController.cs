using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using TimeCapsule.Extentions;
using TimeCapsule.Interfaces;
using TimeCapsule.Services;

namespace TimeCapsule.Controllers
{
    [Authorize]
    public class ProfileController : TimeCapsuleBaseController
    {
        private readonly ProfileService _profileService;
        private readonly IMemoryCache _cache;

        public ProfileController(ILogger<HomeController> logger, ProfileService profileService, IMemoryCache cache)
        {
            _profileService = profileService;
            _cache = cache;
        }

        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> MyCapsules()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Nie można zidentyfikować użytkownika. Prosimy zalogować się ponownie.";
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var result = await _cache.GetOrSetAsync(
                cacheKey: $"mycapsules_{userId}",
                expiration: TimeSpan.FromMinutes(10),
                loadData: () => _profileService.GetMyCapsules(userId));

            return View("~/Views/Home/MyCapsules.cshtml", result);
        }

        [HttpGet]
        [Route("Open/{id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Open(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Nie można zidentyfikować użytkownika. Prosimy zalogować się ponownie.";
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var result = await _profileService.GetCapsuleForOpen(id, userId);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error?.Description;
                return RedirectToAction("MyCapsules", "Profile");
            }

            return View("~/Views/Capsule/OpenedCapsule.cshtml", result.Data);
        }
    }
}



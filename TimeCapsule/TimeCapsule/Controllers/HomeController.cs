using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TimeCapsule.Models;
using TimeCapsule.Models.ViewModels;
using TimeCapsule.Services;

namespace TimeCapsule.Controllers
{
    public class HomeController : TimeCapsuleBaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ContactService _contactService;

        public HomeController(ILogger<HomeController> logger, ContactService contactService)
        {
            _logger = logger;
            _contactService = contactService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

    
        [HttpPost]
        public async Task<IActionResult> SubmitMessage([FromForm] ContactMessageViewModel msg)
        {
            if (!ModelState.IsValid)
            {
                TempData["ContactError"] = "Prosz� wype�ni� wszystkie wymagane pola.";
                return RedirectToAction("Index", "Home");
            }

            var result = await _contactService.SubmitMessage(msg);

            if (result)
            {
                TempData["ContactSuccess"] = "Twoja wiadomo�� zosta�a wys�ana. Dzi�kujemy za kontakt!";    
            }
            else
            {
                TempData["ContactError"] = "Wyst�pi� b��d podczas wysy�ania wiadomo�ci. Spr�buj ponownie p�niej.";
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult FAQ()
        {
            return View();
        }
       

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl ?? "~/");
        }

    }
}

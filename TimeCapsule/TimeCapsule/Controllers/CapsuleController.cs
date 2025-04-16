using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace TimeCapsule.Controllers
{
    [Authorize]
    [Route("TimeCapsule")]
    public class CapsuleController : TimeCapsuleBaseController
    {
        private readonly IEmailSender _emailSender;
        public CapsuleController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Route("Step5")]
        public IActionResult Step5()
        {
            return View("~/Views/Capsule/CreateStep5.cshtml");
        }

        [Route("Step6")]
        public IActionResult Step6()
        {
            return View("~/Views/Capsule/CreateStep6.cshtml");
        }

        [Route("SendEmail")]
        public async Task<IActionResult> SendEmailAsync()
        {
            var receiver = "trybel.anna1@gmail.com";
            var subject = "Test";
            var message = "Test";

            await _emailSender.SendEmailAsync(receiver, subject, message);

            return View("~/Views/Capsule/CreateStep5.cshtml");
        }
    }
}



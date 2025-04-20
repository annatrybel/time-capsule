using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TimeCapsule.Models;
using TimeCapsule.Models.Dto;
using TimeCapsule.Services;

namespace TimeCapsule.Controllers
{
    [Authorize]
    [Route("TimeCapsule")]
    public class CapsuleController : TimeCapsuleBaseController
    {
        private readonly IEmailSender _emailSender;
        private readonly CapsuleService _capsuleService;

        public CapsuleController(IEmailSender emailSender, CapsuleService capsuleService )
        {
            _emailSender = emailSender;
            _capsuleService = capsuleService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Route("Step4")]
        public async Task<IActionResult> Step4([FromForm] CapsuleDto capsule)
        {
            var updatedCapsule = await _capsuleService.GetSectionsWithQuestions(capsule);

            return View("~/Views/Capsule/CreateStep4.cshtml", updatedCapsule);
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

       
        [Route("Step7")]
        public IActionResult Step7()
        {
            return View("~/Views/Capsule/CreateStep7.cshtml");
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



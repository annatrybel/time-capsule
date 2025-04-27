using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using TimeCapsule.Extensions;
using TimeCapsule.Models.DatabaseModels;
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
        //private readonly AttachmentService _attachmentService;

        public CapsuleController(IEmailSender emailSender, CapsuleService capsuleService)
        {
            _emailSender = emailSender;
            _capsuleService = capsuleService;
            //_attachmentService = attachmentService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("Step1")]
        public IActionResult Step1()
        {
            var capsule = HttpContext.Session.GetObject<CreateCapsuleDto>("CurrentCapsule");

            if (capsule == null)
            {
                var newCapsule = new CreateCapsuleDto();

                HttpContext.Session.SetObject("CurrentCapsule", newCapsule);
                return View("~/Views/Capsule/CreateStep1.cshtml", newCapsule);
            }
            return View("~/Views/Capsule/CreateStep1.cshtml", capsule);
        }

        [HttpPost]
        [Route("SaveStep1")]
        public IActionResult SaveStep1([FromForm] CreateCapsuleDto capsule)
        {
            //if (!capsule.Type.HasValue || !Enum.IsDefined(typeof(CapsuleType), capsule.Type.Value))
            //{
            //    ModelState.AddModelError("Type", "Wybór typu kapsuły jest wymagany");
            //    TempData["ErrorMessage"] = "Wybór typu kapsuły jest niezbędny do kontynuowania procesu.";
            //    return View("~/Views/Capsule/CreateStep1.cshtml", capsule);
            //}

            var fullCapsule = HttpContext.Session.GetObject<CreateCapsuleDto>("CurrentCapsule");

            if (fullCapsule != null)
            {
                fullCapsule.Type = capsule.Type;

                if (capsule.Type == CapsuleType.Parna)
                {
                    fullCapsule.Recipients = capsule.Recipients?.Where(r => !string.IsNullOrWhiteSpace(r)).ToList() ?? new List<string>();

                    if (!fullCapsule.Recipients.Any())
                    {
                        ModelState.AddModelError("Recipients", "Należy podać co najmniej jednego odbiorcę");
                        TempData["ErrorMessage"] = "Dla kapsuły parnej należy podać co najmniej jednego odbiorcę.";
                        return View("~/Views/Capsule/CreateStep1.cshtml", capsule);
                    }
                }

                HttpContext.Session.SetObject("CurrentCapsule", fullCapsule);
            }
            else
            {
                TempData["ErrorMessage"] = "Twoja sesja wygasła lub dane zostały utracone. Prosimy rozpocząć proces tworzenia kapsuły od początku.";
                return RedirectToAction("Step1");
            }

            return RedirectToAction("Step2");
        }

        [HttpGet]
        [Route("Step2")]
        public IActionResult Step2()
        {
            var capsule = HttpContext.Session.GetObject<CreateCapsuleDto>("CurrentCapsule");

            if (capsule != null)
            {
                return View("~/Views/Capsule/CreateStep2.cshtml", capsule);
            }

            TempData["ErrorMessage"] = "Twoja sesja wygasła lub dane zostały utracone. Prosimy rozpocząć proces tworzenia kapsuły od początku.";
            return RedirectToAction("Step1");
        }

        [HttpPost]
        [Route("SaveStep2")]
        public IActionResult SaveStep2([FromForm] CreateCapsuleDto capsule)
        {
            //if (!ModelState.IsValid)
            //{
            //	return View("~/Views/Capsule/CreateStep2.cshtml", capsule);
            //}

            if (string.IsNullOrWhiteSpace(capsule.Title))
            {
                ModelState.AddModelError("Title", "Tytuł jest wymagany");
                return View("~/Views/Capsule/CreateStep2.cshtml", capsule);
            }

            var fullCapsule = HttpContext.Session.GetObject<CreateCapsuleDto>("CurrentCapsule");

            if (fullCapsule != null)
            {
                fullCapsule.Title = capsule.Title;
                fullCapsule.Color = capsule.Color;
                fullCapsule.Icon = capsule.Icon;
                HttpContext.Session.SetObject("CurrentCapsule", fullCapsule);
            }
            else
            {
                TempData["ErrorMessage"] = "Twoja sesja wygasła lub dane zostały utracone. Prosimy rozpocząć proces tworzenia kapsuły od początku.";
                return RedirectToAction("Step1");
            }

            return RedirectToAction("Step3");
        }

        [HttpGet]
        [Route("Step3")]
        public IActionResult Step3()
        {
            var capsule = HttpContext.Session.GetObject<CreateCapsuleDto>("CurrentCapsule");

            if (capsule != null)
            {
                return View("~/Views/Capsule/CreateStep3.cshtml", capsule);
            }

            TempData["ErrorMessage"] = "Twoja sesja wygasła lub dane zostały utracone. Prosimy rozpocząć proces tworzenia kapsuły od początku.";
            return RedirectToAction("Step1");
        }

        [HttpPost]
        [Route("SaveStep3")]
        public async Task<IActionResult> SaveStep3([FromForm] CreateCapsuleDto capsule)
        {
            var fullCapsule = HttpContext.Session.GetObject<CreateCapsuleDto>("CurrentCapsule");

            if (fullCapsule != null)
            {
                fullCapsule.Introduction = capsule.Introduction;
                HttpContext.Session.SetObject("CurrentCapsule", fullCapsule);
            }
            else
            {
                TempData["ErrorMessage"] = "Twoja sesja wygasła lub dane zostały utracone. Prosimy rozpocząć proces tworzenia kapsuły od początku.";
                return RedirectToAction("Step1");
            }

            return RedirectToAction("Step4");
        }

        [HttpGet]
        [Route("Step4")]
        public async Task<IActionResult> Step4()
        {
            var capsule = HttpContext.Session.GetObject<CreateCapsuleDto>("CurrentCapsule");

            if (capsule == null)
            {
                TempData["ErrorMessage"] = "Twoja sesja wygasła lub dane zostały utracone. Prosimy rozpocząć proces tworzenia kapsuły od początku.";
                return RedirectToAction("Step1");
            }

            //if (!capsule.Type.HasValue || !Enum.IsDefined(typeof(CapsuleType), capsule.Type.Value))
            //{
            //    TempData["ErrorMessage"] = "Wybór typu kapsuły jest niezbędny do kontynuowania procesu. Prosimy wybrać typ kapsuły.";
            //    return RedirectToAction("Step1");
            //}

            var capsuleWithSections = await _capsuleService.GetSectionsWithQuestions(capsule);

            HttpContext.Session.SetObject("CurrentCapsule", capsuleWithSections);

            return View("~/Views/Capsule/CreateStep4.cshtml", capsuleWithSections);
        }

        [HttpPost]
        [Route("SaveStep4")]
        public async Task<IActionResult> SaveStep4([FromForm] CreateCapsuleDto capsule)
        {
            var fullCapsule = HttpContext.Session.GetObject<CreateCapsuleDto>("CurrentCapsule");

            if (fullCapsule != null)
            {
                fullCapsule.Answers = capsule.Answers ?? new List<CapsuleAnswerDto>();

                HttpContext.Session.SetObject("CurrentCapsule", fullCapsule);
            }
            else
            {
                TempData["ErrorMessage"] = "Twoja sesja wygasła lub dane zostały utracone. Prosimy rozpocząć proces tworzenia kapsuły od początku.";
                return RedirectToAction("Step1");
            }

            return RedirectToAction("Step5");
        }


        [HttpGet]
        [Route("Step5")]
        public IActionResult Step5()
        {
            var capsule = HttpContext.Session.GetObject<CreateCapsuleDto>("CurrentCapsule");
            if (capsule != null)
            {
                return View("~/Views/Capsule/CreateStep5.cshtml", capsule);
            }
            TempData["ErrorMessage"] = "Twoja sesja wygasła lub dane zostały utracone. Prosimy rozpocząć proces tworzenia kapsuły od początku.";

            return RedirectToAction("Step1");
        }

        [HttpPost]
        [Route("SaveStep5")]
        public IActionResult SaveStep5([FromForm] CreateCapsuleDto capsule)
        {
            var fullCapsule = HttpContext.Session.GetObject<CreateCapsuleDto>("CurrentCapsule");

            if (fullCapsule != null)
            {
                fullCapsule.MessageContent = capsule.MessageContent;
                HttpContext.Session.SetObject("CurrentCapsule", fullCapsule);
            }
            else
            {
                TempData["ErrorMessage"] = "Twoja sesja wygasła lub dane zostały utracone. Prosimy rozpocząć proces tworzenia kapsuły od początku.";
                return RedirectToAction("Step1");
            }

            return RedirectToAction("Step6");
        }

        [HttpGet]
        [Route("Step6")]
        public IActionResult Step6()
        {
            var capsule = HttpContext.Session.GetObject<CreateCapsuleDto>("CurrentCapsule");
            if (capsule != null)
            {
                return View("~/Views/Capsule/CreateStep6.cshtml", capsule);
            }
            TempData["ErrorMessage"] = "Twoja sesja wygasła lub dane zostały utracone. Prosimy rozpocząć proces tworzenia kapsuły od początku.";

            return RedirectToAction("Step1");
        }

        [HttpPost]
        [Route("SaveStep6")]
        public async Task<IActionResult> SaveStep6([FromForm] CreateCapsuleDto capsule, List<IFormFile> uploadedFiles)
        {
            var fullCapsule = HttpContext.Session.GetObject<CreateCapsuleDto>("CurrentCapsule");

            if (fullCapsule != null)
            {
                fullCapsule.Links = capsule.Links ?? new List<string>();

                if (uploadedFiles != null && uploadedFiles.Any())
                {
                    if (fullCapsule.UploadedImages == null)
                        fullCapsule.UploadedImages = new List<UploadedImageDto>();

                    foreach (var file in uploadedFiles)
                    {
                        using (var ms = new MemoryStream())
                        {
                            await file.CopyToAsync(ms);
                            var base64 = Convert.ToBase64String(ms.ToArray());

                            fullCapsule.UploadedImages.Add(new UploadedImageDto
                            {
                                FileName = file.FileName,
                                Base64Content = base64,
                                ContentType = file.ContentType
                            });
                        }
                    }
                }
                HttpContext.Session.SetObject("CurrentCapsule", fullCapsule);
            }
            else
            {
                TempData["ErrorMessage"] = "Twoja sesja wygasła lub dane zostały utracone. Prosimy rozpocząć proces tworzenia kapsuły od początku.";
                return RedirectToAction("Step1");
            }

            return RedirectToAction("Step7");
        }

        [HttpGet]
        [Route("Step7")]
        public IActionResult Step7()
        {
            var capsule = HttpContext.Session.GetObject<CreateCapsuleDto>("CurrentCapsule");
            if (capsule != null)
            {
                return View("~/Views/Capsule/CreateStep7.cshtml", capsule);
            }
            TempData["ErrorMessage"] = "Twoja sesja wygasła lub dane zostały utracone. Prosimy rozpocząć proces tworzenia kapsuły od początku.";

            return RedirectToAction("Step1");
        }

        [HttpPost]
        [Route("SaveStep7")]
        public IActionResult SaveStep7([FromForm] CreateCapsuleDto capsule, string OpenDate, string OpenTime, string PredefinedPeriod)
        {
            var fullCapsule = HttpContext.Session.GetObject<CreateCapsuleDto>("CurrentCapsule");

            if (fullCapsule == null)
            {
                TempData["ErrorMessage"] = "Twoja sesja wygasła lub dane zostały utracone. Prosimy rozpocząć proces tworzenia kapsuły od początku.";
                return RedirectToAction("Step1");
            }

            DateTime openingDateTime = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(PredefinedPeriod))
            {
                if (PredefinedPeriod.EndsWith("m"))
                {
                    int months = int.Parse(PredefinedPeriod.TrimEnd('m'));
                    openingDateTime = openingDateTime.AddMonths(months);
                }
                else if (PredefinedPeriod.EndsWith("y"))
                {
                    int years = int.Parse(PredefinedPeriod.TrimEnd('y'));
                    openingDateTime = openingDateTime.AddYears(years);
                }

                openingDateTime = new DateTime(openingDateTime.Year, openingDateTime.Month, openingDateTime.Day, 12, 0, 0, DateTimeKind.Utc);
            }
            else if (!string.IsNullOrEmpty(OpenDate) && !string.IsNullOrEmpty(OpenTime))
            {
                if (DateTime.TryParse($"{OpenDate} {OpenTime}", out DateTime parsedDateTime))
                {
                    if (parsedDateTime <= DateTime.Now)
                    {
                        TempData["ErrorMessage"] = "Data otwarcia kapsuły musi być w przyszłości.";
                        return RedirectToAction("Step7");
                    }

                    openingDateTime = parsedDateTime.ToUniversalTime();
                }
                else
                {
                    TempData["ErrorMessage"] = "Nieprawidłowy format daty lub czasu.";
                    return RedirectToAction("Step7");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Wybierz datę i godzinę otwarcia kapsuły.";
                return RedirectToAction("Step7");
            }

            fullCapsule.OpeningDate = openingDateTime;
            HttpContext.Session.SetObject("CurrentCapsule", fullCapsule);

            return RedirectToAction("Step8");
        }

        [HttpGet]
        [Route("Step8")]
        public IActionResult Step8()
        {
            var capsule = HttpContext.Session.GetObject<CreateCapsuleDto>("CurrentCapsule");
            if (capsule != null)
            {
                return View("~/Views/Capsule/CreateStep8.cshtml", capsule);
            }
            TempData["ErrorMessage"] = "Twoja sesja wygasła lub dane zostały utracone. Prosimy rozpocząć proces tworzenia kapsuły od początku.";

            return RedirectToAction("Step1");
        }

        [HttpPost]
        [Route("SaveStep8")]
        public async Task<IActionResult> SaveStep8([FromForm] CreateCapsuleDto capsule)
        {
            var fullCapsule = HttpContext.Session.GetObject<CreateCapsuleDto>("CurrentCapsule");

            if (fullCapsule == null)
            {
                TempData["ErrorMessage"] = "Twoja sesja wygasła lub dane zostały utracone. Prosimy rozpocząć proces tworzenia kapsuły od początku.";
                return RedirectToAction("Step1");
            }

            //// Walidacja niezbędnych danych
            //if (!fullCapsule.Type.HasValue || string.IsNullOrWhiteSpace(fullCapsule.Title) || fullCapsule.OpeningDate == default)
            //{
            //    TempData["ErrorMessage"] = "Brakuje niektórych wymaganych danych. Prosimy uzupełnić wszystkie wymagane pola.";
            //    return RedirectToAction("Step8");
            //}

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Nie można zidentyfikować użytkownika. Prosimy zalogować się ponownie.";
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var result = await _capsuleService.SaveCapsule(fullCapsule, userId);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error?.Description ?? "Wystąpił błąd podczas zapisywania kapsuły.";
                return RedirectToAction("Step8");
            }

            TempData["SuccessMessage"] = "Kapsuła została zapisana pomyślnie!";

            HttpContext.Session.SetObject("CurrentCapsule", fullCapsule);

            return RedirectToAction("Step9");
        }

        [HttpGet]
        [Route("Step9")]
        public IActionResult Step9()   //ToDo zabić sesje
        {
            var capsule = HttpContext.Session.GetObject<CreateCapsuleDto>("CurrentCapsule");
            if (capsule != null)
            {
                return View("~/Views/Capsule/CreateStep9.cshtml", capsule);
            }
            TempData["ErrorMessage"] = "Twoja sesja wygasła lub dane zostały utracone. Prosimy rozpocząć proces tworzenia kapsuły od początku.";

            return RedirectToAction("Step1");
        }
    }
}





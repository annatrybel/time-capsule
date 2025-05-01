using Microsoft.AspNetCore.Mvc;
using TimeCapsule.Models.Dto;
using TimeCapsule.Services.Results;
using TimeCapsule.Services;
using Microsoft.AspNetCore.Authorization;

namespace TimeCapsule.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("AdminPanel/Forms")]
    public class FormManagementController : TimeCapsuleBaseController
    {
        private readonly FormManagementService _formManagementService;

        public FormManagementController(FormManagementService formManagementService)
        {
            _formManagementService = formManagementService;
        }

        public async Task<IActionResult> GetForms()
        {
            var sectionsResult = await _formManagementService.GetFormSectionsWithQuestions();

            if (!sectionsResult.IsSuccess)
            {
                return View("~/Views/AdminPanel/Forms/FormsManagementView.cshtml", new List<CapsuleSectionDto>());
            }

            return View("~/Views/AdminPanel/Forms/FormsManagementView.cshtml", sectionsResult.Data);
        }

        [HttpPost("UpdateQuestion")]
        public async Task<IActionResult> UpdateQuestion(int QuestionId, string QuestionText)
        {
            if (QuestionId <= 0 || string.IsNullOrWhiteSpace(QuestionText))
            {
                TempData["ErrorMessage"] = "Należy podać poprawny identyfikator pytania i treść.";
                return RedirectToAction("Forms");
            }

            var result = await _formManagementService.UpdateQuestion(QuestionId, QuestionText);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Pytanie zostało zaktualizowane pomyślnie.";
            }

            return RedirectToAction("Forms");
        }

        [HttpPost("AddSection")]
        public async Task<IActionResult> AddSection([FromForm] CreateSectionDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult.Failure("Invalid section data"));
            }

            var result = await _formManagementService.AddSection(model);
            return RedirectToAction("Forms");
        }

        [HttpPost("AddQuestion")]
        public async Task<IActionResult> AddQuestion([FromForm] CreateQuestionDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult.Failure("Invalid q data"));
            }

            var result = await _formManagementService.AddQuestion(model);
            return RedirectToAction("Forms");
        }

        [HttpPost("DeleteQuestion/{questionId}")]
        public async Task<IActionResult> DeleteQuestion(int questionId)
        {
            if (questionId <= 0)
            {
                TempData["ErrorMessage"] = "Nieprawidłowy identyfikator pytania.";
                return RedirectToAction("Forms");
            }

            var result = await _formManagementService.DeleteQuestion(questionId);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Pytanie zostało usunięte pomyślnie.";
            }

            return RedirectToAction("Forms");
        }
    }
}

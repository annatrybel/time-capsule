using Microsoft.AspNetCore.Mvc;
using TimeCapsule.Models.Dto;
using TimeCapsule.Services.Results;
using TimeCapsule.Services;
using Microsoft.AspNetCore.Authorization;
using TimeCapsule.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TimeCapsule.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("AdminPanel/Forms")]
    public class FormManagementController : TimeCapsuleBaseController
    {
        private readonly IFormManagementService _formManagementService;

        public FormManagementController(IFormManagementService formManagementService)
        {
            _formManagementService = formManagementService;
        }

        [HttpGet]
        public async Task<IActionResult> GetForms()
        {
            var sectionsResult = await _formManagementService.GetFormSectionsWithQuestions();

            if (!sectionsResult.IsSuccess)
            {
                return View("~/Views/AdminPanel/Forms/FormsManagementView.cshtml", new List<CapsuleSectionDto>());
            }

            return View("~/Views/AdminPanel/Forms/FormsManagementView.cshtml", sectionsResult.Data);
        }

        
        [HttpPost("AddSection")]
        public async Task<IActionResult> AddSection([FromForm] CreateSectionDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult.Failure("Nieprawidłowe dane sekcji"));
            }

            var result = await _formManagementService.AddSection(model);
            return RedirectToAction("GetForms");
        }

        [HttpGet("GetSectionById/{sectionId}")]
        public async Task<IActionResult> GetSectionById(int sectionId)
        {
            if (sectionId <= 0)
            {
                return BadRequest(ServiceResult.Failure("Nieprawidłowy parametr: ID sekcji musi być większe od zera"));
            }
            var result = await _formManagementService.GetSectionById(sectionId);
            return HandleStatusCodeServiceResult(result);
        }

        [HttpPost("UpdateSection")]
        public async Task<IActionResult> UpdateSection([FromForm] UpdateSectionDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult.Failure("Nieprawidłowe dane sekcji"));
            }

            var result = await _formManagementService.UpdateSection(model);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Sekcja została zaktualizowana pomyślnie.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Error.Description;
            }

            return RedirectToAction("GetForms");
        }


        [HttpPost("AddQuestion")]
        public async Task<IActionResult> AddQuestion([FromForm] UpsertQuestionDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult.Failure("Nieprawidłowe dane"));
            }

            var result = await _formManagementService.AddQuestion(model);
            return RedirectToAction("GetForms");
        }

        [HttpGet("GetQuestionById/{questionId}")]
        public async Task<IActionResult> GetQuestionById(int questionId)
        {
            if (questionId <= 0)
            {
                return BadRequest(ServiceResult.Failure("Nieprawidłowy parametr: ID pytania musi być większe od zera"));
            }

            var question = await _formManagementService.GetQuestionById(questionId);
            return HandleStatusCodeServiceResult(question);
        }

        [HttpPost("UpdateQuestion")]
        public async Task<IActionResult> UpdateQuestion([FromForm] UpsertQuestionDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult.Failure("Nieprawidłowe dane"));
            }

            var result = await _formManagementService.UpdateQuestion(model);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Pytanie zostało zaktualizowane pomyślnie.";
            }

            return RedirectToAction("GetForms");
        }

        [HttpPost("DeleteQuestion/{questionId}")]
        public async Task<IActionResult> DeleteQuestion(int questionId)
        {
            if (questionId <= 0)
            {
                TempData["ErrorMessage"] = "Nieprawidłowy identyfikator pytania.";
                return RedirectToAction("GetForms");
            }

            var result = await _formManagementService.DeleteQuestion(questionId);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Pytanie zostało usunięte pomyślnie.";
            }

            return RedirectToAction("GetForms");
        }
    }
}

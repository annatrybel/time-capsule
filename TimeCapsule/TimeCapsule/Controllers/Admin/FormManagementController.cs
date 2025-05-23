using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeCapsule.Interfaces;
using TimeCapsule.Models.Dto;
using TimeCapsule.Services.Results;
using static System.Collections.Specialized.BitVector32;

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
        [ApiExplorerSettings(IgnoreApi = true)]
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
                var errors = ModelState.Values
                       .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                       .ToList();
                return BadRequest(ServiceResult.Failure("Invalid data:\n" + string.Join("\n", errors)));
            }
            var result = await _formManagementService.AddSection(model);

            if(result.IsSuccess)
            {
                TempData["SuccessMessage"] = $"Section {model.SectionName} created successfully";
                TempData["SuccessMessageId"] = $"section_create__{DateTime.UtcNow.Ticks}";
                return HandleServiceResult(result);
            }
            else
            {
                return BadRequest(ServiceResult.Failure(result.Error.Description));
            }
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
                var errors = ModelState.Values
                       .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                       .ToList();
                return BadRequest(ServiceResult.Failure("Invalid data", string.Join("\n", errors)));
            }
            var result = await _formManagementService.UpdateSection(model);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = $"Section {model.SectionName} updated successfully";
                TempData["SuccessMessageId"] = $"section_update_{model.SectionId}_{DateTime.UtcNow.Ticks}";
                return HandleServiceResult(result);
            }
            else
            {
                return BadRequest(ServiceResult.Failure(result.Error.Description));
            }
        }


        [HttpPost("AddQuestion")]
        public async Task<IActionResult> AddQuestion([FromForm] CreateQuestionDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                       .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                       .ToList();
                return BadRequest(ServiceResult.Failure("Invalid data", string.Join("\n", errors)));
            }
            var result = await _formManagementService.AddQuestion(model);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = $"Question created successfully";
                TempData["SuccessMessageId"] = $"question_create_{DateTime.UtcNow.Ticks}";
                return HandleServiceResult(result);
            }
            else
            {
                return BadRequest(ServiceResult.Failure(result.Error.Description));
            }
        }

        [HttpGet("GetQuestionById/{questionId}")]
        public async Task<IActionResult> GetQuestionById(int questionId)
        {
            if (questionId <= 0)
            {
                return BadRequest(ServiceResult.Failure("Nieprawidłowy parametr: ID pytania musi być większe od zera"));
            }

            var result = await _formManagementService.GetQuestionById(questionId);
            return HandleStatusCodeServiceResult(result);
        }

        [HttpPost("UpdateQuestion")]
        public async Task<IActionResult> UpdateQuestion([FromForm] UpdateQuestionDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                       .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                       .ToList();
                return BadRequest(ServiceResult.Failure("Invalid data", string.Join("\n", errors)));
            }
            var result = await _formManagementService.UpdateQuestion(model);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = $"Question updated successfully";
                TempData["SuccessMessageId"] = $"question_update_{model.Id}_{DateTime.UtcNow.Ticks}";
                return HandleServiceResult(result);
            }
            else
            {
                return BadRequest(ServiceResult.Failure(result.Error.Description));
            }
        }

        [HttpPost("DeleteQuestion/{questionId}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteQuestion(int questionId)
        {
            if (questionId <= 0)
            {
                return BadRequest(ServiceResult.Failure("Nieprawidłowy identyfikator pytania."));
            }
            var result = await _formManagementService.DeleteQuestion(questionId);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = $"Question deleted successfully";
                TempData["SuccessMessageId"] = $"question_delete_{questionId}_{DateTime.UtcNow.Ticks}";
                return RedirectToAction("GetForms");
            }
            else
            {
                return BadRequest(ServiceResult.Failure(result.Error.Description));
            }
        }

        [HttpPost("DeleteSection/{sectionId}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteSection(int sectionId)
        {
            if (sectionId <= 0)
            {
                return BadRequest(ServiceResult.Failure("Nieprawidłowy identyfikator sekcji."));
            }

            var result = await _formManagementService.DeleteSection(sectionId);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = $"Section deleted successfully";
                TempData["SuccessMessageId"] = $"section_delete_{sectionId}_{DateTime.UtcNow.Ticks}";
                return RedirectToAction("GetForms");
            }
            else
            {
                return BadRequest(ServiceResult.Failure(result.Error.Description));
            }
        }
    }
}

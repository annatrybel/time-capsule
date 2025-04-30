using TimeCapsule.Models.Dto;
using TimeCapsule.Services.Results;

namespace TimeCapsule.Interfaces
{
    public interface IFormManagementService
    {
        Task<ServiceResult<List<CapsuleSectionDto>>> GetFormSectionsWithQuestions();
        Task<ServiceResult> AddSection(CreateSectionDto model);
        Task<ServiceResult> AddQuestion(CreateQuestionDto model);
        Task<ServiceResult> UpdateQuestion(int questionId, string questionText);
        Task<ServiceResult> DeleteQuestion(int questionId);
    }
}

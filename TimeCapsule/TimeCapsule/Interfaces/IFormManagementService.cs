using TimeCapsule.Models.Dto;
using TimeCapsule.Services.Results;

namespace TimeCapsule.Interfaces
{
    public interface IFormManagementService
    {
        Task<ServiceResult<List<CapsuleSectionDto>>> GetFormSectionsWithQuestions();
        Task<ServiceResult> AddSection(CreateSectionDto model);
        Task<ServiceResult> UpdateSection(UpdateSectionDto model);
        Task<ServiceResult<CapsuleSectionDto>> GetSectionById(int sectionId);
        Task<ServiceResult> AddQuestion(UpsertQuestionDto model);
        Task<ServiceResult<CapsuleQuestionDto>> GetQuestionById(int questionId);
        Task<ServiceResult> UpdateQuestion(UpsertQuestionDto model);
        Task<ServiceResult> DeleteQuestion(int questionId);
        Task<ServiceResult> DeleteSection(int sectionId);
    }
}

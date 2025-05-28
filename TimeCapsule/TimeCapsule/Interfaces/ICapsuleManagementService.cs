using Microsoft.AspNetCore.Identity;
using TimeCapsule.Models;
using TimeCapsule.Models.DatabaseModels;
using TimeCapsule.Models.Dto;
using TimeCapsule.Models.ViewModels;
using TimeCapsule.Services.Results;

namespace TimeCapsule.Interfaces
{
    public interface ICapsuleManagementService
    {
        Task<ServiceResult<DataTableResponse<CapsuleManagementViewModel>>> GetCapsules(DataTableRequest request);
        Task<ServiceResult> UpdateCapsuleOpeningDate(UpdateCapsuleOpeningDateDto model);
        Task<ServiceResult<CapsuleOpeningDateDto>> GetCapsuleOpeningDate(int capsuleId);
        Task<ServiceResult<CapsuleRecipientsDto>> GetCapsuleRecipients(int capsuleId);
        Task<ServiceResult> UpdateCapsuleRecipients(UpdateCapsuleRecipientsDto model, IdentityUser user);


    }
}



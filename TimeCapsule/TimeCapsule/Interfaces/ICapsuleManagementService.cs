using TimeCapsule.Models;
using TimeCapsule.Models.Dto;
using TimeCapsule.Models.ViewModels;
using TimeCapsule.Services.Results;

namespace TimeCapsule.Interfaces
{
    public interface ICapsuleManagementService
    {
        Task<ServiceResult<DataTableResponse<CapsuleManagementViewModel>>> GetCapsules(DataTableRequest request);
    }
}



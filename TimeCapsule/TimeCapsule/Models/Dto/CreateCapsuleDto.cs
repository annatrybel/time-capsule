using Microsoft.AspNetCore.Identity;
using TimeCapsule.Models.DatabaseModels;
using System.ComponentModel.DataAnnotations;

namespace TimeCapsule.Models.Dto
{
    public class CreateCapsuleDto
    {
        public int? Id { get; set; }
        public CapsuleType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public List<string> Recipients { get; set; } = new List<string>();
        public bool NotifyRecipients { get; set; }
        public string? Icon { get; set; }
        public string? Color { get; set; }
        public string? Introduction { get; set; }
        public List<CapsuleSectionDto> CapsuleSections { get; set; } = new List<CapsuleSectionDto>();
        public List<CapsuleAnswerDto>? Answers { get; set; } = new List<CapsuleAnswerDto>();
        public string? MessageContent { get; set; }
        public List<string> Links { get; set; } = new List<string>();
        public List<UploadedImageDto>? UploadedImages { get; set; } = new List<UploadedImageDto>();
        public DateTime OpeningDate { get; set; }
        public string? CreatedByUserId { get; set; }
        public IdentityUser CreatedByUser { get; set; } 
        public Status Status { get; set; }

    }
}



using System.ComponentModel.DataAnnotations;
using TimeCapsule.Models.DatabaseModels;

namespace TimeCapsule.Models.Dto
{
    public class CreateSectionDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string SectionName { get; set; }
        [Required]
        public CapsuleType CapsuleType { get; set; }
    }
}


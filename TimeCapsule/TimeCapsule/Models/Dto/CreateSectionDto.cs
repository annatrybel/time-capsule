using System.ComponentModel.DataAnnotations;

namespace TimeCapsule.Models.Dto
{
    public class CreateSectionDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string SectionName { get; set; }        
    }
}


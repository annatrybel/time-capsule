using System.ComponentModel.DataAnnotations;
using TimeCapsule.Models.DatabaseModels;

namespace TimeCapsule.Models.Dto
{
    public class CapsuleSectionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
        public CapsuleType CapsuleType { get; set; }
        public List<CapsuleQuestionDto> Questions { get; set; } = new List<CapsuleQuestionDto>();
    }

    public class CreateSectionDto
    {
        [Required(ErrorMessage = "Nazwa sekcji jest wymagana")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Nazwa sekcji musi mieć od 3 do 100 znaków")]
        public string SectionName { get; set; }

        [Required(ErrorMessage = "Typ kapsuły jest wymagany")]
        public CapsuleType CapsuleType { get; set; }
    }
}

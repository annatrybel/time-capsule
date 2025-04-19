using System.ComponentModel.DataAnnotations;

namespace TimeCapsule.Models.Dto
{
    public class CreateQuestionDto
    {
        [Required]
        public int SectionId { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 3)]
        public string QuestionText { get; set; } = string.Empty;
    }
}

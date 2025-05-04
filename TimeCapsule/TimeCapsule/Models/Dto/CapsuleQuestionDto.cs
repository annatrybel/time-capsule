using System.ComponentModel.DataAnnotations;

namespace TimeCapsule.Models.Dto
{
    public class CapsuleQuestionDto
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }
        public int SectionId { get; set; }
        public string SectionName { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class CreateQuestionDto
    {
        [Required(ErrorMessage = "Sekcja jest wymagana")]
        public int SectionId { get; set; }

        [Required(ErrorMessage = "Treść pytania jest wymagana")]
        public string QuestionText { get; set; } = string.Empty;
    }
    public class UpdateQuestionDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Treść pytania jest wymagana")]
        public string QuestionText { get; set; } = string.Empty;
    }
}

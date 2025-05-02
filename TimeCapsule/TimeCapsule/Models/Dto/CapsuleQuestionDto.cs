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

    public class UpsertQuestionDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Sekcja jest wymagana")]
        public int SectionId { get; set; }

        [Required(ErrorMessage = "Treść pytania jest wymagana")]
        [StringLength(500, MinimumLength = 3, ErrorMessage = "Treść pytania musi mieć od 3 do 500 znaków")]
        public string QuestionText { get; set; } = string.Empty;
    }
}

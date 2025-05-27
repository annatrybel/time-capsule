using System.ComponentModel.DataAnnotations;


namespace TimeCapsule.Models.Dto
{
    public class CapsuleRecipientsDto
    {
        public int CapsuleId { get; set; }
        public string Title { get; set; }
        public List<string> Recipients { get; set; } = new List<string>();
    }

    public class UpdateCapsuleRecipientsDto
    {
        [Required(ErrorMessage = "Identyfikator kapsuły jest wymagany")]
        public int CapsuleId { get; set; }

        [Required(ErrorMessage = "Lista odbiorców nie może być pusta")]
        public List<string> Recipients { get; set; } = new List<string>();
    }

}



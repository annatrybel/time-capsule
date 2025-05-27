using System.ComponentModel.DataAnnotations;

namespace TimeCapsule.Models.Dto
{
    public class CapsuleOpeningDateDto
    {
        public int CapsuleId { get; set; }
        public string Title { get; set; }
        public DateTime CurrentOpeningDate { get; set; }
    }

    public class UpdateCapsuleOpeningDateDto
    {
        [Required(ErrorMessage = "Identyfikator kapsuły jest wymagany")]
        public int CapsuleId { get; set; }

        [Required(ErrorMessage = "Nowa data otwarcia jest wymagana")]
        public DateTime NewOpeningDate { get; set; }
    }
}



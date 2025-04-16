using Microsoft.AspNetCore.Identity;
using TimeCapsule.Models.DatabaseModels;

namespace TimeCapsule.Models.Dto
{
    public class CapsuleDto
    {
        public int Id { get; set; }
        public CapsuleType Type { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public string Introduction { get; set; }
        public CapsuleQuestion CapsuleQuestions { get; set; }
        public CapsuleAnswer CapsuleAnswers { get; set; }
        public string MessageContent { get; set; }
        public DateTime OpeningDate { get; set; }
        public string CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public IdentityUser CreatedByUser { get; set; }
        public Status Status { get; set; }
    }
}



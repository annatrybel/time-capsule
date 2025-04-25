using System.ComponentModel.DataAnnotations.Schema;

namespace TimeCapsule.Models.DatabaseModels
{
    public class CapsuleQuestion
    {
        public int Id { get; set; }

        public string QuestionText { get; set; }

        public int CapsuleSectionId { get; set; }

        public int DisplayOrder { get; set; }

        public virtual CapsuleSection CapsuleSection { get; set; }

        public virtual ICollection<CapsuleAnswer> CapsuleAnswers { get; set; } = new List<CapsuleAnswer>();
    }
}

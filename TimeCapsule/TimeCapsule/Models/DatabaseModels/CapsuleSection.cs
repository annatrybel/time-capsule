using System.ComponentModel.DataAnnotations;

namespace TimeCapsule.Models.DatabaseModels
{
    public class CapsuleSection
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public CapsuleType CapsuleType { get; set; }

        public int DisplayOrder { get; set; }

        public virtual ICollection<CapsuleQuestion> Questions { get; set; } = new List<CapsuleQuestion>();
    }
}

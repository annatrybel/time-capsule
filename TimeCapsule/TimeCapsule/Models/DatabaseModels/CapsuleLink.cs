using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TimeCapsule.Models.DatabaseModels
{
    public class CapsuleLink
    {
        public int Id { get; set; }

        public int CapsuleId { get; set; }

        public string Url { get; set; }

        [ForeignKey("CapsuleId")]
        public virtual Capsule Capsule { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimeCapsule.Models.DatabaseModels
{
    public class CapsuleRecipient
    {
        public int Id { get; set; }

        public int CapsuleId { get; set; }

        public string Email { get; set; }

        public bool EmailSent { get; set; } = false;

        public virtual Capsule Capsule { get; set; }
    }
}
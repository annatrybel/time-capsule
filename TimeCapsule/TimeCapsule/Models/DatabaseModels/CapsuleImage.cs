using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TimeCapsule.Models.DatabaseModels
{
    public class CapsuleImage
    {
        public int Id { get; set; }
        public int CapsuleId { get; set; }
        public string FileName { get; set; }
        public byte[] Content { get; set; }
        public Capsule Capsule { get; set; }
    }  
}

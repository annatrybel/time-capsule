using TimeCapsule.Models.DatabaseModels;

namespace TimeCapsule.Models.ViewModels
{
    public class CapsuleManagementViewModel
    {
        public int Id { get; set; }
        public string OwnerId { get; set; }
        public string OwnerName { get; set; }
        public List<string> Recipients { get; set; }
        public CapsuleType CapsuleType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime OpeningDate { get; set; }
        public Status Status { get; set; }
    }
}




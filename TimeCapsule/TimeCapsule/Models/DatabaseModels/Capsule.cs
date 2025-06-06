﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TimeCapsule.Models.DatabaseModels
{
    public class Capsule
    {
        public int Id { get; set; }
        public CapsuleType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string? Color { get; set; }
        public string? Introduction { get; set; }
        public string? MessageContent { get; set; }
        public DateTime OpeningDate { get; set; }
        public string CreatedByUserId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public IdentityUser CreatedByUser { get; set; }
        public Status Status { get; set; }


        public ICollection<CapsuleRecipient> CapsuleRecipients { get; set; } = new List<CapsuleRecipient>();
        public ICollection<CapsuleAnswer> CapsuleAnswers { get; set; } = new List<CapsuleAnswer>();
        public ICollection<CapsuleImage> CapsuleImages { get; set; } = new List<CapsuleImage>();
        public ICollection<CapsuleLink> CapsuleLinks { get; set; } = new List<CapsuleLink>();
    }
    public enum CapsuleType
    {
        Indywidualna = 0,
        [Display(Name = "Dla kogoś")]
        DlaKogos = 1
    }

    public enum Status
    {
        Created,
        Opened,
        Deactivated
    }
}














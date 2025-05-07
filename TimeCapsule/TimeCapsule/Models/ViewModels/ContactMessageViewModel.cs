using System.ComponentModel.DataAnnotations;

namespace TimeCapsule.Models.ViewModels
{
    public class ContactMessageViewModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string? Message { get; set; } 
    }
}

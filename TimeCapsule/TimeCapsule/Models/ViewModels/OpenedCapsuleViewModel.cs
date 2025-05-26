using TimeCapsule.Models.DatabaseModels;

namespace TimeCapsule.Models.ViewModels
{
    public class OpenedCapsuleViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public string Introduction { get; set; }
        public string MessageContent { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime OpeningDate { get; set; }
        public string CreatedByName { get; set; }
        public CapsuleType Type { get; set; }

        public List<OpenedCapsuleSectionViewModel> Sections { get; set; } = new List<OpenedCapsuleSectionViewModel>();
        public List<OpenedCapsuleImageViewModel> Images { get; set; } = new List<OpenedCapsuleImageViewModel>();
        public List<string> Links { get; set; } = new List<string>();
    }

    public class OpenedCapsuleSectionViewModel
    {
        public int SectionId { get; set; }
        public string SectionName { get; set; }
        public int DisplayOrder { get; set; }
        public List<OpenedCapsuleQuestionViewModel> Questions { get; set; } = new List<OpenedCapsuleQuestionViewModel>();
    }

    public class OpenedCapsuleQuestionViewModel
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public int DisplayOrder { get; set; }
        public string Answer { get; set; }
    }

    public class OpenedCapsuleImageViewModel
    {
        public string FileName { get; set; }
        public string Content { get; set; }
        public string ContentType { get; set; }
    }
}



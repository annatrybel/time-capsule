namespace TimeCapsule.Models.Dto
{
    public class CapsuleSectionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
        public List<CapsuleQuestionDto> Questions { get; set; } = new List<CapsuleQuestionDto>();
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeCapsule.Models;
using TimeCapsule.Models.Dto;
using static System.Collections.Specialized.BitVector32;

namespace TimeCapsule.Services
{
    public class CapsuleService
    {
        private readonly TimeCapsuleContext _context;
        private readonly ILogger<CapsuleService> _logger;

        public CapsuleService(TimeCapsuleContext context, ILogger<CapsuleService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CreateCapsuleDto> GetSectionsWithQuestions(CreateCapsuleDto capsule)
        {
            try
            {
                var sections = await _context.CapsuleSections
                    .Include(s => s.Questions)
                    .Where(s => s.CapsuleType == capsule.Type)
                    .OrderBy(s => s.DisplayOrder)
                    .ToListAsync();

                var sectionDtos = sections.Select(s => new CapsuleSectionDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    DisplayOrder = s.DisplayOrder,
                    CapsuleType = s.CapsuleType,
                    Questions = s.Questions
                        .OrderBy(q => q.DisplayOrder)
                        .Select(q => new CapsuleQuestionDto
                        {
                            Id = q.Id,
                            QuestionText = q.QuestionText,
                            SectionId = s.Id,
                            SectionName = s.Name,
                            DisplayOrder = q.DisplayOrder
                        }).ToList()
                }).ToList();

                capsule.CapsuleSections = sectionDtos;

                _logger.LogInformation("Loaded {SectionCount} sections with {QuestionCount} questions for capsule type {CapsuleType}",
                    sectionDtos.Count,
                    sectionDtos.Sum(s => s.Questions.Count),
                    capsule.Type);

                return capsule;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading questions for capsule type {CapsuleType}", capsule.Type);
                return new CreateCapsuleDto();
            }
        }
    }
}


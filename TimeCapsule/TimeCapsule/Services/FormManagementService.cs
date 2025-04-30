using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TimeCapsule.Models.Dto;
using TimeCapsule.Models;
using TimeCapsule.Models.DatabaseModels;
using TimeCapsule.Services.Results;
using TimeCapsule.Interfaces;

namespace TimeCapsule.Services
{
    public class FormManagementService : IFormManagementService
    {
        private readonly TimeCapsuleContext _context;
        private readonly ILogger<FormManagementService> _logger;
        public FormManagementService(TimeCapsuleContext context, ILogger<FormManagementService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult<List<CapsuleSectionDto>>> GetFormSectionsWithQuestions()
        {
            try
            {
                var sections = await _context.CapsuleSections
                    .Include(s => s.Questions)
                    .OrderBy(s => s.DisplayOrder)
                    .ToListAsync();

                var sectionDtos = sections.Select(s => new CapsuleSectionDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    CapsuleType = s.CapsuleType,
                    DisplayOrder = s.DisplayOrder,
                    Questions = s.Questions
                        .OrderBy(q => q.DisplayOrder)
                        .Select(q => new CapsuleQuestionDto
                        {
                            Id = q.Id,
                            QuestionText = q.QuestionText,
                            DisplayOrder = q.DisplayOrder
                        }).ToList()
                }).ToList();

                return ServiceResult<List<CapsuleSectionDto>>.Success(sectionDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching form sections and questions");
                return ServiceResult<List<CapsuleSectionDto>>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<ServiceResult> AddSection(CreateSectionDto model)
        {
            try
            {
                var query = await _context.CapsuleSections
                    .Select(s => new
                    {
                        NameExists = s.Name.ToLower() == model.SectionName.ToLower(),
                        DisplayOrder = s.DisplayOrder
                    })
                    .ToListAsync();

                if (query.Any(s => s.NameExists))
                {
                    return ServiceResult.Failure("Sekcja o takiej nazwie już istnieje");
                }

                int newDisplayOrder = 1;
                if (query.Any())
                {
                    newDisplayOrder = query.Max(s => s.DisplayOrder) + 1;
                }

                var section = new CapsuleSection
                {
                    Name = model.SectionName,
                    DisplayOrder = newDisplayOrder,
                    CapsuleType = model.CapsuleType
                };

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    _context.CapsuleSections.Add(section);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation("Added new section: {SectionName} with order {DisplayOrder}",
                        model.SectionName, newDisplayOrder);

                    return ServiceResult.Success();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding new section: {SectionName}", model.SectionName);
                return ServiceResult.Failure($"Nie udało się dodać sekcji: {ex.Message}");
            }
        }

        public async Task<ServiceResult> AddQuestion(CreateQuestionDto model)
        {
            try
            {
                var section = await _context.CapsuleSections
                    .Include(s => s.Questions)
                    .FirstOrDefaultAsync(s => s.Id == model.SectionId);

                if (section == null)
                {
                    return ServiceResult.Failure("Section not found");
                }

                int newDisplayOrder = 1;
                if (section.Questions.Any())
                {
                    newDisplayOrder = section.Questions.Max(q => q.DisplayOrder) + 1;
                }

                var question = new CapsuleQuestion
                {
                    QuestionText = model.QuestionText,
                    CapsuleSectionId = model.SectionId,
                    DisplayOrder = newDisplayOrder
                };
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    _context.CapsuleQuestions.Add(question);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation("Added new question to section {SectionId}: {QuestionText}",
                        model.SectionId, model.QuestionText);

                    return ServiceResult.Success();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding question to section {SectionId}", model.SectionId);
                return ServiceResult.Failure($"Failed to add question: {ex.Message}");
            }
        }

        public async Task<ServiceResult> UpdateQuestion(int questionId, string questionText)
        {
            try
            {
                var question = await _context.CapsuleQuestions
                    .FirstOrDefaultAsync(q => q.Id == questionId);

                if (question == null)
                {
                    return ServiceResult.Failure("Nie znaleziono pytania");
                }

                question.QuestionText = questionText;

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    _context.CapsuleQuestions.Update(question);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation("Zaktualizowano pytanie {QuestionId}: {QuestionText}",
                        questionId, questionText);

                    return ServiceResult.Success();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas aktualizacji pytania {QuestionId}", questionId);
                return ServiceResult.Failure($"Nie udało się zaktualizować pytania: {ex.Message}");
            }
        }

        public async Task<ServiceResult> DeleteQuestion(int questionId)
        {
            try
            {
                var question = await _context.CapsuleQuestions
                    .Include(q => q.CapsuleAnswers)
                    .FirstOrDefaultAsync(q => q.Id == questionId);

                if (question == null)
                {
                    return ServiceResult.Failure("Nie znaleziono pytania o podanym ID");
                }

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    if (question.CapsuleAnswers != null && question.CapsuleAnswers.Any())
                    {
                        _context.CapsuleAnswers.RemoveRange(question.CapsuleAnswers);
                    }

                    _context.CapsuleQuestions.Remove(question);

                    await _context.SaveChangesAsync();

                    var questionsToUpdate = await _context.CapsuleQuestions
                        .Where(q => q.CapsuleSectionId == question.CapsuleSectionId && q.DisplayOrder > question.DisplayOrder)
                        .OrderBy(q => q.DisplayOrder)
                        .ToListAsync();

                    foreach (var q in questionsToUpdate)
                    {
                        q.DisplayOrder--;
                        _context.CapsuleQuestions.Update(q);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation("Usunięto pytanie {QuestionId} z sekcji {SectionId}",
                        questionId, question.CapsuleSectionId);

                    return ServiceResult.Success();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas usuwania pytania {QuestionId}", questionId);
                return ServiceResult.Failure($"Nie udało się usunąć pytania: {ex.Message}");
            }
        }
    }
}

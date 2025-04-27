using Microsoft.EntityFrameworkCore;
using TimeCapsule.Models;
using TimeCapsule.Models.Dto;
using TimeCapsule.Services.Results;
using TimeCapsule.Models.DatabaseModels;
using Microsoft.AspNetCore.Http.HttpResults;

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

        public async Task<ServiceResult<int>> SaveCapsule(CreateCapsuleDto capsuleDto, string userId)
        {
            try
            {
                if (capsuleDto.OpeningDate == default || capsuleDto.OpeningDate == DateTime.MinValue)
                {
                    return ServiceResult<int>.Failure("Data otwarcia kapsuły jest wymagana i musi być prawidłową datą w przyszłości.");
                }

                if (capsuleDto.OpeningDate <= DateTime.UtcNow)
                {
                    return ServiceResult<int>.Failure("Data otwarcia kapsuły musi być w przyszłości.");
                }

                var capsule = new Capsule
                {
                    CreatedByUserId = userId,
                    Title = capsuleDto.Title,
                    Type = capsuleDto.Type,
                    Icon = capsuleDto.Icon,
                    Color = capsuleDto.Color,
                    Introduction = capsuleDto.Introduction,
                    MessageContent = capsuleDto.MessageContent,
                    OpeningDate = capsuleDto.OpeningDate.ToUniversalTime(),
                    Status = Status.Created
                };

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    _context.Capsules.Add(capsule);
                    await _context.SaveChangesAsync();

                    if (capsuleDto.Answers != null && capsuleDto.Answers.Any())
                    {
                        foreach (var answerDto in capsuleDto.Answers.Where(a => !string.IsNullOrWhiteSpace(a.AnswerText)))
                        {
                            var answer = new CapsuleAnswer
                            {
                                CapsuleId = capsule.Id,
                                QuestionId = answerDto.QuestionId,
                                AnswerText = answerDto.AnswerText
                            };
                            _context.CapsuleAnswers.Add(answer);
                        }
                        await _context.SaveChangesAsync();
                    }

                    // Zapis zdjęć
                    if (capsuleDto.UploadedImages != null && capsuleDto.UploadedImages.Any())
                    {
                        foreach (var imageDto in capsuleDto.UploadedImages)
                        {
                            var image = new CapsuleImage
                            {
                                CapsuleId = capsule.Id,
                                FileName = imageDto.FileName,
                                Content = Convert.FromBase64String(imageDto.Base64Content)
                            };
                            _context.CapsuleImages.Add(image);
                        }
                        await _context.SaveChangesAsync();
                    }

                    // Zapis linków
                    if (capsuleDto.Links != null && capsuleDto.Links.Any())
                    {
                        foreach (var link in capsuleDto.Links.Where(l => !string.IsNullOrWhiteSpace(l)))
                        {
                            var capsuleLink = new CapsuleLink
                            {
                                CapsuleId = capsule.Id,
                                Url = link
                            };
                            _context.CapsuleLinks.Add(capsuleLink);
                        }
                        await _context.SaveChangesAsync();
                    }

                    // Zapis odbiorców dla kapsuł parnych
                    if (capsuleDto.Type == CapsuleType.Parna &&
                        capsuleDto.Recipients != null &&
                        capsuleDto.Recipients.Any())
                    {
                        foreach (var email in capsuleDto.Recipients.Where(e => !string.IsNullOrWhiteSpace(e)))
                        {
                            var recipient = new CapsuleRecipient
                            {
                                CapsuleId = capsule.Id,
                                Email = email
                            };
                            _context.CapsuleRecipients.Add(recipient);
                        }
                        await _context.SaveChangesAsync();
                    }

                    // Zatwierdzenie transakcji
                    await transaction.CommitAsync();

                    _logger.LogInformation("Kapsuła {CapsuleId} utworzona pomyślnie przez użytkownika {UserId}", capsule.Id, userId);
                    return ServiceResult<int>.Success(capsule.Id);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Błąd podczas zapisywania kapsuły dla użytkownika {UserId}", userId);
                    return ServiceResult<int>.Failure("Wystąpił błąd podczas zapisywania kapsuły. Spróbuj ponownie później.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Nieoczekiwany błąd podczas zapisywania kapsuły dla użytkownika {UserId}", userId);
                return ServiceResult<int>.Failure("Wystąpił nieoczekiwany błąd. Spróbuj ponownie później.");
            }
        }
    }
}


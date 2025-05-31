using Microsoft.EntityFrameworkCore;
using TimeCapsule.Models;
using TimeCapsule.Models.DatabaseModels;
using TimeCapsule.Models.ViewModels;
using TimeCapsule.Services.Results;

namespace TimeCapsule.Services
{
    public class ProfileService
    {
        private readonly TimeCapsuleContext _context;
        private readonly ILogger<CapsuleService> _logger;

        public ProfileService(TimeCapsuleContext context, ILogger<CapsuleService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<TimeRemainingViewModel>> GetMyCapsules(string userId)
        {
            try
            {
                var capsules = await _context.Capsules
                    .Where(u => u.CreatedByUserId == userId)
                    .OrderBy(u => u.OpeningDate)
                    .ToListAsync();

                var result = new List<TimeRemainingViewModel>();

                foreach (var capsule in capsules)
                {
                    var timeRemaining = capsule.OpeningDate - DateTime.UtcNow;
                    bool canOpen = timeRemaining.TotalSeconds <= 0;

                    var myCapsule = new TimeRemainingViewModel
                    {
                        Id = capsule.Id,
                        Title = capsule.Title,
                        Years = Math.Max(0, timeRemaining.Days / 365),
                        Days = Math.Max(0, timeRemaining.Days % 365),
                        Hours = Math.Max(0, timeRemaining.Hours),
                        Minutes = Math.Max(0, timeRemaining.Minutes),
                        CanOpen = canOpen
                    };
                    result.Add(myCapsule);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas pobierania kapsuł użytkownika");
                return new List<TimeRemainingViewModel>();
            }
        }

        public async Task<ServiceResult<OpenedCapsuleViewModel>> GetCapsuleForOpen(int capsuleId, string userId)
        {
            try
            {
                var capsule = await _context.Capsules
                    .Include(c => c.CreatedByUser)
                    .Include(c => c.CapsuleRecipients)
                    .Include(c => c.CapsuleImages)
                    .Include(c => c.CapsuleLinks)
                    .Include(c => c.CapsuleAnswers)
                        .ThenInclude(a => a.CapsuleQuestion)
                            .ThenInclude(q => q.CapsuleSection)
                    .FirstOrDefaultAsync(c => c.Id == capsuleId);

                if (capsule == null)
                {
                    return ServiceResult<OpenedCapsuleViewModel>.Failure("Kapsuła o podanym identyfikatorze nie istnieje.");
                }

                if (capsule.OpeningDate > DateTime.UtcNow)
                {
                    return ServiceResult<OpenedCapsuleViewModel>.Failure("Ta kapsuła nie jest jeszcze dostępna do otwarcia.");
                }

                bool isCreator = capsule.CreatedByUserId == userId;
                bool isRecipient = false;
                if (capsule.Type == CapsuleType.DlaKogos)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                    if (user != null && !string.IsNullOrEmpty(user.Email))
                    {
                        isRecipient = capsule.CapsuleRecipients.Any(cr => cr.Email == user.Email);
                    }
                }

                if (!isCreator && !isRecipient)
                {
                    return ServiceResult<OpenedCapsuleViewModel>.Failure("Nie masz uprawnień do otwarcia tej kapsuły.");
                }

                if (capsule.Status == Status.Created)
                {
                    capsule.Status = Status.Opened;
                    await _context.SaveChangesAsync();
                }

                var viewModel = new OpenedCapsuleViewModel
                {
                    Id = capsule.Id,
                    Title = capsule.Title,
                    Icon = capsule.Icon,
                    Color = capsule.Color,
                    Introduction = capsule.Introduction,
                    MessageContent = capsule.MessageContent,
                    CreatedAt = capsule.CreatedAt,
                    OpeningDate = capsule.OpeningDate,
                    CreatedByName = capsule.CreatedByUser?.UserName ?? "Anonim",
                    Type = capsule.Type,

                    Sections = capsule.CapsuleAnswers
                        .Where(a => a.CapsuleQuestion != null)
                        .GroupBy(a => a.CapsuleQuestion.CapsuleSection)
                        .Select(g => new OpenedCapsuleSectionViewModel
                        {
                            SectionId = g.Key.Id,
                            SectionName = g.Key.Name,
                            DisplayOrder = g.Key.DisplayOrder,
                            Questions = g.Select(a => new OpenedCapsuleQuestionViewModel
                            {
                                QuestionId = a.CapsuleQuestion.Id,
                                QuestionText = a.CapsuleQuestion.QuestionText,
                                DisplayOrder = a.CapsuleQuestion.DisplayOrder,
                                Answer = a.AnswerText
                            }).OrderBy(q => q.DisplayOrder).ToList()
                        }).OrderBy(s => s.DisplayOrder).ToList(),


                    Images = capsule.CapsuleImages.Select(i => new OpenedCapsuleImageViewModel
                    {
                        FileName = i.FileName,
                        Content = Convert.ToBase64String(i.Content),
                        ContentType = GetContentTypeFromFileName(i.FileName)
                    }).ToList(),


                    Links = capsule.CapsuleLinks.Select(l => l.Url).ToList()
                };

                _logger.LogInformation("Kapsuła {CapsuleId} została otwarta przez użytkownika {UserId}", capsuleId, userId);
                return ServiceResult<OpenedCapsuleViewModel>.Success(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas otwierania kapsuły {CapsuleId} dla użytkownika {UserId}", capsuleId, userId);
                return ServiceResult<OpenedCapsuleViewModel>.Failure("Wystąpił błąd podczas otwierania kapsuły. Spróbuj ponownie później.");
            }
        }

        private string GetContentTypeFromFileName(string fileName)
        {
            string ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                _ => "application/octet-stream"
            };
        }
    }
}







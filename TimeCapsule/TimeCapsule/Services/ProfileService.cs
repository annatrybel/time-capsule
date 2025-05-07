using Microsoft.EntityFrameworkCore;
using TimeCapsule.Models;
using TimeCapsule.Models.ViewModels;

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
    }
}





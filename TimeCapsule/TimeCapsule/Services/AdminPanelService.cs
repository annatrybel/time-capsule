using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeCapsule.Models;
using TimeCapsule.Models.DatabaseModels;
using TimeCapsule.Models.Dto;
using TimeCapsule.Services.Results;

namespace TimeCapsule.Services
{
    public class AdminPanelService 
    {
        private readonly TimeCapsuleContext _context;
        private readonly ILogger<AdminPanelService> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        public AdminPanelService(TimeCapsuleContext context, ILogger<AdminPanelService> logger, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<ServiceResult<DataTableResponse<UserDto>>> GetUsers(DataTableRequest request)
        {
            try
            {
                string[] columnNames = { "UserName", "Email", "Role" };
                string sortColumn = (request.OrderColumn >= 0 && request.OrderColumn < columnNames.Length) ? columnNames[request.OrderColumn] : "UserName";
                string sortDirection = request.OrderDir?.ToUpper() == "DESC" ? "DESC" : "ASC";

                var filteredUsers = await _context.Set<UserDto>()
                    .FromSqlInterpolated($"SELECT * FROM public.get_users_with_roles({request.SearchValue ?? (object)DBNull.Value}, {sortColumn}, {sortDirection})")
                    .ToListAsync();

                int totalRecords = await _context.Users.CountAsync();
                int filteredRecords = filteredUsers.Count;

                return ServiceResult<DataTableResponse<UserDto>>.Success(
                    new DataTableResponse<UserDto>
                    {
                        Draw = request.Draw,
                        RecordsTotal = totalRecords,
                        RecordsFiltered = filteredRecords,
                        Data = filteredUsers
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching users with roles: {ErrorDetails}", ex.ToString());
                return ServiceResult<DataTableResponse<UserDto>>.Failure($"Error fetching data: {ex.Message}");
            }
        }


        public async Task<ServiceResult> LockUser(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return ServiceResult.Failure("User not found");

                await _userManager.SetLockoutEnabledAsync(user, true);

                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);

                _logger.LogInformation("User {UserId} locked permanently", userId);
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error permanently locking user {UserId}", userId);
                return ServiceResult.Failure($"Failed to lock user: {ex.Message}");
            }
        }

        public async Task<ServiceResult> UnlockUser(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return ServiceResult.Failure("User not found");

                await _userManager.SetLockoutEndDateAsync(user, null);

                _logger.LogInformation("User {UserId} unlocked", userId);
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unlocking user {UserId}", userId);
                return ServiceResult.Failure($"Failed to unlock user: {ex.Message}");
            }
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
                catch(Exception ex)
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
    }
}


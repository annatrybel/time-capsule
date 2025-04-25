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

        public async Task<ServiceResult<UserDto>> CreateUser(UserDto user)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existing = await _userManager.FindByEmailAsync(user.Email);
                if (existing != null)
                    return ServiceResult<UserDto>.Failure($"User {user.Email} already exists");

                var role = await _context.Roles.FindAsync(user.RoleId);
                if (role == null)
                    return ServiceResult<UserDto>.Failure($"Role with ID {user.RoleId} not found");

                var newUser = new IdentityUser
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    EmailConfirmed = true
                };

                var createResult = await _userManager.CreateAsync(newUser, "DefaultPassword123!");
                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    _logger.LogError("Failed to create user: {Errors}", errors);
                    return ServiceResult<UserDto>.Failure($"Failed to create user: {errors}");
                }

                var roleResult = await _userManager.AddToRoleAsync(newUser, role.Name);
                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    _logger.LogError("Failed to add user to role: {Errors}", errors);
                    return ServiceResult<UserDto>.Failure($"Failed to add user to role: {errors}");
                }

                await transaction.CommitAsync();

                user.UserId = newUser.Id;
                return ServiceResult<UserDto>.Success(user);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating user {Email}", user.Email);
                return ServiceResult<UserDto>.Failure($"Failed to save user: {ex.Message}");
            }
        }


        public async Task<ServiceResult> UpdateUser(UserDto model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                    return ServiceResult.Failure("User not found");

                var role = await _context.Roles.FindAsync(model.RoleId);
                if (role == null)
                    return ServiceResult.Failure($"Role with ID {model.RoleId} not found");

                user.UserName = model.UserName;
                user.Email = model.Email;

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                    _logger.LogError("Failed to update user: {Errors}", errors);
                    return ServiceResult.Failure($"Failed to update user: {errors}");
                }

                var currentRoles = await _userManager.GetRolesAsync(user);
                if (currentRoles.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                }
                await _userManager.AddToRoleAsync(user, role.Name);

                await transaction.CommitAsync();
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Error updating roles for user {model.UserId}");
                return ServiceResult.Failure("Error updating user roles");
            }
        }

        public async Task<ServiceResult<UserDto>> GetUserById(string userId)
        {
            try
            {
                var userData = await _context.Users
                     .Where(u => u.Id == userId)
                     .Join(_context.UserRoles,
                           u => u.Id,
                           ur => ur.UserId,
                           (u, ur) => new { User = u, UserRole = ur })
                     .Join(_context.Roles,
                           ur => ur.UserRole.RoleId,
                           r => r.Id,
                           (ur, r) => new
                           {
                               User = ur.User,
                               Role = r
                           })
                     .FirstOrDefaultAsync();

                if (userData == null)
                    return ServiceResult<UserDto>.Failure($"User with ID {userId} not found");

                var userDto = new UserDto
                {
                    UserId = userData.User.Id,
                    UserName = userData.User.UserName,
                    Email = userData.User.Email,
                    RoleId = userData.Role.Id,
                    RoleName = userData.Role.Name
                };

                return ServiceResult<UserDto>.Success(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user with ID {userId}");
                return ServiceResult<UserDto>.Failure("Error retrieving user details");
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


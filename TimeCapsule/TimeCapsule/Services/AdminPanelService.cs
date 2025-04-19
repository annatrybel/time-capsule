using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeCapsule.Models;
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
    }
}


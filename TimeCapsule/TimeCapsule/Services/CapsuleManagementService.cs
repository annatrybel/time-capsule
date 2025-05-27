using Microsoft.EntityFrameworkCore;
using System.Web;
using TimeCapsule.Interfaces;
using TimeCapsule.Models;
using TimeCapsule.Models.DatabaseModels;
using TimeCapsule.Models.ViewModels;
using TimeCapsule.Services.Results;
using System.Linq.Dynamic.Core;
using TimeCapsule.Models.Dto;

namespace TimeCapsule.Services
{
    public class CapsuleManagementService : ICapsuleManagementService
    {
        private readonly TimeCapsuleContext _context;
        private readonly ILogger<UserManagementService> _logger;
        public CapsuleManagementService(TimeCapsuleContext context, ILogger<UserManagementService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<ServiceResult<DataTableResponse<CapsuleManagementViewModel>>> GetCapsules(DataTableRequest request)
        {
            try
            {
                string[] columnNames = { "ownerName", "recipients", "capsuleType", "createdAt", "openingDate", "status", "id" };
                string sortColumn = (request.OrderColumn >= 0 && request.OrderColumn < columnNames.Length) ? columnNames[request.OrderColumn] : "openingDate";

                IQueryable<Capsule> baseQuery = _context.Capsules
                    .Include(c => c.CreatedByUser)
                    .Include(c => c.CapsuleRecipients);

                var query = baseQuery.Select(c => new CapsuleManagementViewModel
                {
                    Id = c.Id,
                    OwnerId = c.CreatedByUserId,
                    OwnerName = c.CreatedByUser.UserName,
                    Recipients = c.CapsuleRecipients.Select(cr => cr.Email).ToList(),
                    CapsuleType = c.Type,
                    CreatedAt = c.CreatedAt,
                    OpeningDate = c.OpeningDate,
                    Status = c.Status
                }).AsQueryable();

                var totalRecords = await query.CountAsync();

                // Wyszukiwanie
                if (!string.IsNullOrEmpty(request.SearchValue))
                {
                    string searchValueLower = request.SearchValue.ToLower();
                    query = query.Where(c =>
                        c.OwnerName.ToLower().Contains(searchValueLower) ||
                        c.Recipients.Any(r => r.ToLower().Contains(searchValueLower))
                    );
                }

                var recordsFiltered = await query.CountAsync();

                // Sortowanie
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(request.OrderDir)))
                {
                    if (sortColumn.ToLower() == "recipients")
                    {
                        query = request.OrderDir.ToLower() == "asc" ?
                            query.OrderBy(c => c.Recipients.FirstOrDefault()) :
                            query.OrderByDescending(c => c.Recipients.FirstOrDefault());
                    }
                    else
                    {
                        query = query.OrderBy(sortColumn + " " + request.OrderDir);
                    }
                }
                else
                {
                    query = query.OrderBy(c => c.OpeningDate);
                }

                var data = await query.Skip(request.Start).Take(request.Length).ToListAsync();

                return ServiceResult<DataTableResponse<CapsuleManagementViewModel>>.Success(new DataTableResponse<CapsuleManagementViewModel>
                {
                    Draw = request.Draw,
                    RecordsTotal = totalRecords,
                    RecordsFiltered = recordsFiltered,
                    Data = data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas pobierania danych kapsuł");
                return ServiceResult<DataTableResponse<CapsuleManagementViewModel>>.Failure("Wystąpił błąd podczas pobierania danych kapsuł.");
            }
        }

        public async Task<ServiceResult<CapsuleOpeningDateDto>> GetCapsuleOpeningDate(int capsuleId)
        {
            try
            {
                var capsule = await _context.Capsules
                    .Where(c => c.Id == capsuleId)
                    .Select(c => new CapsuleOpeningDateDto
                    {
                        CapsuleId = c.Id,
                        CurrentOpeningDate = c.OpeningDate,
                        Title = c.Title
                    })
                    .FirstOrDefaultAsync();

                if (capsule == null)
                {
                    return ServiceResult<CapsuleOpeningDateDto>.Failure("Kapsuła o podanym identyfikatorze nie istnieje.");
                }

                return ServiceResult<CapsuleOpeningDateDto>.Success(capsule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas pobierania daty otwarcia kapsuły {CapsuleId}", capsuleId);
                return ServiceResult<CapsuleOpeningDateDto>.Failure("Wystąpił błąd podczas pobierania daty otwarcia kapsuły.");
            }
        }

        public async Task<ServiceResult> UpdateCapsuleOpeningDate(UpdateCapsuleOpeningDateDto model)
        {
            try
            {
                var capsule = await _context.Capsules.FindAsync(model.CapsuleId);

                if (capsule == null)
                {
                    return ServiceResult.Failure("Kapsuła o podanym identyfikatorze nie istnieje.");
                }

                if (model.NewOpeningDate <= DateTime.UtcNow)
                {
                    return ServiceResult.Failure("Data otwarcia kapsuły musi być w przyszłości.");
                }

                capsule.OpeningDate = model.NewOpeningDate;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Zaktualizowano datę otwarcia kapsuły {CapsuleId} na {OpeningDate}",
                    model.CapsuleId, model.NewOpeningDate);

                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas aktualizacji daty otwarcia kapsuły {CapsuleId}", model.CapsuleId);
                return ServiceResult.Failure("Wystąpił błąd podczas aktualizacji daty otwarcia kapsuły.");
            }
        }
    }
}





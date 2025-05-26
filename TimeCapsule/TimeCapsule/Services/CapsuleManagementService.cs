using Microsoft.EntityFrameworkCore;
using System.Web;
using TimeCapsule.Interfaces;
using TimeCapsule.Models;
using TimeCapsule.Models.DatabaseModels;
using TimeCapsule.Models.ViewModels;
using TimeCapsule.Services.Results;
using System.Linq.Dynamic.Core;

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
    }
}



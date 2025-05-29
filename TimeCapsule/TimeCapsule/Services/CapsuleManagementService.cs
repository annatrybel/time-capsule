using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Linq.Dynamic.Core;
using TimeCapsule.Interfaces;
using TimeCapsule.Models;
using TimeCapsule.Models.DatabaseModels;
using TimeCapsule.Models.Dto;
using TimeCapsule.Models.ViewModels;
using TimeCapsule.Services.Results;

namespace TimeCapsule.Services
{
    public class CapsuleManagementService : ICapsuleManagementService
    {
        private readonly TimeCapsuleContext _context;
        private readonly ILogger<UserManagementService> _logger;
        private readonly CapsuleService _capsuleService;
        private readonly IEmailSender _emailSender;
        public CapsuleManagementService(TimeCapsuleContext context, ILogger<UserManagementService> logger, CapsuleService capsuleService, IEmailSender emailSender)
        {
            _context = context;
            _logger = logger;
            _capsuleService = capsuleService;
            _emailSender = emailSender;
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
        public async Task<ServiceResult<CapsuleRecipientsDto>> GetCapsuleRecipients(int capsuleId)
        {
            try
            {
                var capsule = await _context.Capsules
                    .Include(c => c.CapsuleRecipients)
                    .Where(c => c.Id == capsuleId)
                    .Select(c => new CapsuleRecipientsDto
                    {
                        CapsuleId = c.Id,
                        Title = c.Title,
                        Recipients = c.CapsuleRecipients.Select(r => new
                        CapsuleRecipient
                        {
                            Email = r.Email,
                            EmailSent = r.EmailSent
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (capsule == null)
                {
                    return ServiceResult<CapsuleRecipientsDto>.Failure("Kapsuła o podanym identyfikatorze nie istnieje.");
                }

                return ServiceResult<CapsuleRecipientsDto>.Success(capsule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas pobierania odbiorców kapsuły {CapsuleId}", capsuleId);
                return ServiceResult<CapsuleRecipientsDto>.Failure("Wystąpił błąd podczas pobierania odbiorców kapsuły.");
            }
        }

        public async Task<ServiceResult> UpdateCapsuleRecipients(UpdateCapsuleRecipientsDto model, IdentityUser user)
        {
            try
            {
                var capsule = await _context.Capsules
                    .Include(c => c.CapsuleRecipients)
                    .FirstOrDefaultAsync(c => c.Id == model.CapsuleId);

                if (capsule == null)
                {
                    return ServiceResult.Failure("Kapsuła o podanym identyfikatorze nie istnieje.");
                }

                if (capsule.Type != CapsuleType.DlaKogos)
                {
                    return ServiceResult.Failure("Tylko kapsuły typu 'Dla kogoś' mogą mieć odbiorców.");
                }

                var newRecipientEmailsFromModel = model.Recipients                           
                    .Where(email => !string.IsNullOrWhiteSpace(email))                      
                    .Select(email => email.Trim().ToLowerInvariant())                       
                    .Distinct()                                                             
                    .ToList(); 

                if (newRecipientEmailsFromModel.Count == 0 && model.Recipients.Any(originalEmail => !string.IsNullOrWhiteSpace(originalEmail)))
                {
                    return ServiceResult.Failure("Żaden z podanych adresów email nie jest poprawny.");
                }

                var existingDbRecipients = capsule.CapsuleRecipients.ToList();

                var emailsSentSuccessfullyThisUpdate = new List<string>();
                var emailsFailedToSendThisUpdate = new List<string>();

                bool hasAnyDbChanges = false; 

                var recipientsToRemove = existingDbRecipients
                    .Where(er => !newRecipientEmailsFromModel.Contains(er.Email.ToLowerInvariant()))
                    .ToList();

                if (recipientsToRemove.Any())
                {
                    _context.CapsuleRecipients.RemoveRange(recipientsToRemove);
                    _logger.LogInformation("Usunięto {Count} odbiorców z kapsuły {CapsuleId}: {RemovedEmails}",
                        recipientsToRemove.Count, capsule.Id, string.Join(", ", recipientsToRemove.Select(r => r.Email)));
                    hasAnyDbChanges = true; 
                }

                string senderName = null;
                if (model.NotifyRecipients) 
                {
                    senderName = user?.UserName ?? "Twój Znajomy";
                }

                foreach (var emailModel in newRecipientEmailsFromModel)
                {
                    var existingRecipientInDb = existingDbRecipients
                        .FirstOrDefault(er => er.Email.ToLowerInvariant() == emailModel);

                    if (existingRecipientInDb != null)
                    {
                        if (model.NotifyRecipients && !existingRecipientInDb.EmailSent)
                        {
                            _logger.LogInformation("Próba wysyłki do istniejącego odbiorcy {Email} (EmailSent=false) dla kapsuły {CapsuleId}", existingRecipientInDb.Email, capsule.Id);
                            bool emailSent = await this.TrySendNotificationAndMarkAsSentAsync(existingRecipientInDb, capsule.Title, capsule.OpeningDate, senderName);
                            if (emailSent)
                            {
                                emailsSentSuccessfullyThisUpdate.Add(existingRecipientInDb.Email);
                                hasAnyDbChanges = true; 
                                _logger.LogInformation("Wysłano e-mail i (domniemanie) oznaczono EmailSent=true dla {Email}", existingRecipientInDb.Email);
                            }
                            else
                            {
                                emailsFailedToSendThisUpdate.Add(existingRecipientInDb.Email);
                            }
                        }
                    }
                    else 
                    {
                        var newCapsuleRecipient = new CapsuleRecipient
                        {
                            CapsuleId = capsule.Id,
                            Email = emailModel,
                            EmailSent = false 
                        };
                        capsule.CapsuleRecipients.Add(newCapsuleRecipient);
                        _logger.LogInformation("Dodano nowego odbiorcę {Email} do kapsuły {CapsuleId}.", emailModel, capsule.Id);
                        hasAnyDbChanges = true; 

                        if (model.NotifyRecipients)
                        {
                            _logger.LogInformation("Próba wysyłki do nowego odbiorcy {Email} dla kapsuły {CapsuleId}", newCapsuleRecipient.Email, capsule.Id);
                            bool emailSent = await this.TrySendNotificationAndMarkAsSentAsync(newCapsuleRecipient, capsule.Title, capsule.OpeningDate, senderName);
                            if (emailSent)
                            {
                                emailsSentSuccessfullyThisUpdate.Add(newCapsuleRecipient.Email);
                                _logger.LogInformation("Wysłano e-mail i (domniemanie) oznaczono EmailSent=true dla nowego odbiorcy {Email}", newCapsuleRecipient.Email);
                            }
                            else
                            {
                                emailsFailedToSendThisUpdate.Add(newCapsuleRecipient.Email);
                            }
                        }
                    }
                }

                if (hasAnyDbChanges)
                {
                    try
                    {
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Zapisano zmiany (strukturalne i/lub statusy EmailSent) dla kapsuły {CapsuleId}.", capsule.Id);
                    }
                    catch (DbUpdateException dbEx)
                    {
                        _logger.LogError(dbEx, "Błąd bazy danych podczas zapisywania zmian dla kapsuły {CapsuleId}.", model.CapsuleId);
                        string failureMessage = "Błąd bazy danych podczas aktualizacji odbiorców.";
                        if (emailsSentSuccessfullyThisUpdate.Any() || emailsFailedToSendThisUpdate.Any())
                        { 
                            failureMessage += " Statusy wysłanych/nieudanych e-maili mogły nie zostać zapisane.";
                        }
                        if (emailsFailedToSendThisUpdate.Any())
                        { 
                            failureMessage += $" Nie udało się wysłać e-maili do: {string.Join(", ", emailsFailedToSendThisUpdate)}.";
                        }
                        return ServiceResult.Failure(failureMessage);
                    }
                }
                else if (!hasAnyDbChanges && model.NotifyRecipients && !emailsFailedToSendThisUpdate.Any() && !emailsSentSuccessfullyThisUpdate.Any())
                {
                    _logger.LogInformation("NotifyRecipients=true, ale brak odbiorców wymagających powiadomienia lub zmian statusu (np. wszyscy już otrzymali e-mail lub brak kwalifikujących się odbiorców) dla kapsuły {CapsuleId}.", capsule.Id);
                }

                if (emailsFailedToSendThisUpdate.Any())
                {
                    return ServiceResult.SuccessWithWarning($"Zaktualizowano odbiorców. Nie udało się wysłać e-maili do: {string.Join(", ", emailsFailedToSendThisUpdate)}.");
                }

                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd ogólny podczas aktualizacji odbiorców kapsuły {CapsuleId}", model.CapsuleId);
                return ServiceResult.Failure("Wystąpił błąd podczas aktualizacji odbiorców kapsuły.");
            }
        }

        private async Task<bool> TrySendNotificationAndMarkAsSentAsync(CapsuleRecipient recipient, string capsuleTitle, DateTime openingDate, string senderName)
        {
            if (recipient == null || string.IsNullOrWhiteSpace(recipient.Email))
            {
                return false;
            }

            if (recipient.EmailSent)
            {
                _logger.LogInformation(
                    "Email do odbiorcy {Email} był już wcześniej wysłany. Pomijanie.",
                    recipient.Email);
                return true; 
            }

            try
            {
                string subject = $"Zostałeś dodany/a jako odbiorca kapsuły czasu od {senderName ?? "znajomego"}!";
                string message = await _capsuleService.GenerateEmailTemplateAsync(senderName, capsuleTitle, openingDate); 

                await _emailSender.SendEmailAsync(recipient.Email, subject, message);
                recipient.EmailSent = true; 

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Błąd podczas wysyłania powiadomienia o kapsule do odbiorcy {Email}. Status EmailSent nie został zmieniony.", recipient.Email);
                return false;
            }
        }

        public async Task<ServiceResult> DeactivateCapsule(int capsuleId)
        {
            try
            {
                var capsule = await _context.Capsules.FindAsync(capsuleId);

                if (capsule == null)
                {
                    return ServiceResult.Failure("Kapsuła o podanym identyfikatorze nie istnieje.");
                }

                if (capsule.Status == Status.Deactivated)
                {
                    return ServiceResult.Failure("Kapsuła jest już dezaktywowana.");
                }

                capsule.Status = Status.Deactivated;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Dezaktywowano kapsułę {CapsuleId}", capsuleId);

                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas dezaktywacji kapsuły {CapsuleId}", capsuleId);
                return ServiceResult.Failure("Wystąpił błąd podczas dezaktywacji kapsuły.");
            }
        }
    }
}





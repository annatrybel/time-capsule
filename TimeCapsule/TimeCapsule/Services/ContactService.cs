﻿using Microsoft.AspNetCore.Identity.UI.Services;
using TimeCapsule.Models;
using TimeCapsule.Models.ViewModels;

namespace TimeCapsule.Services
{
    public class ContactService
    {
        private readonly TimeCapsuleContext _context;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ContactService> _logger;
        private readonly IConfiguration _configuration;

        public ContactService(
            TimeCapsuleContext context,
            IEmailSender emailSender,
            ILogger<ContactService> logger,
            IConfiguration configuration)
        {
            _context = context;
            _emailSender = emailSender;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<bool> SubmitMessage(ContactMessageViewModel message)
        {
            try
            {
                string subject = "Zapytanie ze strony TimeCapsule";

                string adminEmail = _configuration["EmailConfiguration:From"];
                string emailSubject = $"Nowa wiadomość kontaktowa: {subject}";
                string emailBody = $@"
                	<h2>Nowa wiadomość kontaktowa</h2>
                	<p><strong>Od:</strong> {message.Name} ({message.Email})</p>
                	<p><strong>Temat:</strong> {subject}</p>
                	<p><strong>Wiadomość:</strong></p>
                	<p>{message.Message}</p>
                	<hr>
                	<p>Ta wiadomość została wysłana z formularza kontaktowego TimeCapsule.</p>
            	";

                await _emailSender.SendEmailAsync(adminEmail, emailSubject, emailBody);

                // potwierdzenie do nadawcy
                string confirmationSubject = "Potwierdzenie otrzymania wiadomości - TimeCapsule";
                string confirmationBody = $@"
                	<h2>Dziękujemy za kontakt!</h2>
                	<p>Otrzymaliśmy Twoją wiadomość i odpowiemy na nią jak najszybciej.</p>
                	<p><strong>Temat:</strong> {subject}</p>
                	<p><strong>Treść Twojej wiadomości:</strong></p>
                	<p>{message.Message}</p>
                	<hr>
                	<p>Z wyrazami szacunku,<br>Zespół TimeCapsule</p>
            	";

                await _emailSender.SendEmailAsync(message.Email, confirmationSubject, confirmationBody);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas wysyłania wiadomości kontaktowej");
                return false;
            }
        }
    }
}

﻿using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace TimeCapsule.Services
{
    public class EmailSender : IEmailSender
    {
        public IConfiguration config { get; }
        public EmailSender(IConfiguration config)
        {
            this.config = config;
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using (MailMessage mailMessage = new MailMessage(config["EmailConfiguration:From"], email))
            {
                mailMessage.Subject = subject;
                mailMessage.Body = htmlMessage;
                mailMessage.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient();
                smtp.Host = config["EmailConfiguration:SmtpServer"];
                smtp.EnableSsl = bool.Parse(config["EmailConfiguration:EnableSsl"]);
                NetworkCredential networkCredentials = new NetworkCredential(config["EmailConfiguration:From"], config["EmailConfiguration:Password"]);
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = networkCredentials;
                smtp.Port = int.Parse(config["EmailConfiguration:Port"]);

                await smtp.SendMailAsync(mailMessage);
            }
        }
    }
}

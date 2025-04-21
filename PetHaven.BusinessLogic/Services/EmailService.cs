using Microsoft.Extensions.Configuration;
using PetHaven.BusinessLogic.Interfaces;
using Microsoft.Extensions.Logging;
using PetHaven.Data.Model;
using System.Net.Mail;
using System.Net;
using PetHaven.BusinessLogic.Templates;

namespace PetHaven.BusinessLogic.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        private async Task SendEmailAsync(string to, string subject, string htmlMessage)
        {
            try
            {
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]!);
                var smtpUsername = _configuration["EmailSettings:SmtpUsername"];
                var smtpPassword = _configuration["EmailSettings:SmtpPassword"];
                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                var senderName = _configuration["EmailSettings:SenderName"];

                using (var client = new SmtpClient(smtpServer, smtpPort))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                    client.EnableSsl = true;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(senderEmail, senderName),
                        Subject = subject,
                        Body = htmlMessage,
                        IsBodyHtml = true
                    };

                    //mailMessage.To.Add(to);
                    mailMessage.To.Add("olawaledavid11@gmail.com");

                    await client.SendMailAsync(mailMessage);
                    _logger.LogInformation($"Email sent to {to} successfully");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send email: {ex.Message}");
                throw;
            }
        }

        public async Task SendSignupConfirmationAsync(User user)
        {
            var subject = "Welcome to PeHaven - Your Account Has Been Created";
            var message = GetSignupEmailTemplate(user);
            await SendEmailAsync(user.Email, subject, message);
        }

        public async Task SendMedicationNotificationAsync(User petOwner, Pet pet, Medication medication)
        {
            var subject = $"New Medication Added for {pet.Name}";
            var message = GetMedicationEmailTemplate(petOwner, pet, medication);
            await SendEmailAsync(petOwner.Email, subject, message);
        }

        public async Task SendImmunizationNotificationAsync(User petOwner, Pet pet, Immunization immunization)
        {
            var subject = $"New Immunization Record for {pet.Name}";
            var message = GetImmunizationEmailTemplate(petOwner, pet, immunization);
            await SendEmailAsync(petOwner.Email, subject, message);
        }

        private string GetSignupEmailTemplate(User user)
        {
            return SignupEmailTemplate.GetTemplate(user);
        }

        private string GetMedicationEmailTemplate(User petOwner, Pet pet, Medication medication)
        {
            return MedicationEmailTemplate.GetTemplate(petOwner, pet, medication);
        }

        private string GetImmunizationEmailTemplate(User petOwner, Pet pet, Immunization immunization)
        {
            return ImmunizationEmailTemplate.GetTemplate(petOwner, pet, immunization);
        }
    }
}

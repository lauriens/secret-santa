using Microsoft.Extensions.Options;
using SecretSanta.Models;
using SecretSanta.Options;
using System.Net.Mail;

namespace SecretSanta.Services
{
    public interface IEmailService
    {
        Task SendEmail(Friend friend);
    }

    public class EmailService : IEmailService
    {
        private readonly ILogger<IEmailService> logger;
        private readonly MailClientOptions mailClientOptions;

        public EmailService(ILogger<IEmailService> logger, IOptions<MailClientOptions> mailClientOptions)
        {
            this.logger = logger;
            this.mailClientOptions = mailClientOptions.Value;
        }

        public async Task SendEmail(Friend friend)
        {
            logger.LogInformation("Send email process started");

            MailMessage newMail = new();

            SmtpClient client = new("smtp.office365.com");

            // Follow the RFS 5321 Email Standard
            newMail.From = new MailAddress(mailClientOptions.User, "Amigo Secreto da Família");

            newMail.To.Add(friend.Email);// declare the email subject

            newMail.Subject = $"{friend.Name}, seu amigo secreto é..."; // use HTML for the email body

            newMail.IsBodyHtml = true;

            string htmlBody = $"{friend.SecretSanta!.Name}";
            newMail.Body = htmlBody;

            // enable SSL for encryption across channels
            client.EnableSsl = true;
            // Port 465 for SSL communication
            client.Port = 587;
            client.UseDefaultCredentials = false;
            // Provide authentication information with Gmail SMTP server to authenticate your sender account
            client.Credentials = new System.Net.NetworkCredential(mailClientOptions.User, mailClientOptions.Password);

            await client.SendMailAsync(newMail); // Send the constructed mail

            logger.LogInformation("Send email process ended");
        }
    }
}

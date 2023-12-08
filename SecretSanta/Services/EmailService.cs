using Microsoft.Extensions.Options;
using SecretSanta.Models;
using SecretSanta.Options;
using System.Drawing;
using System.Net.Mail;
using System.Net.Mime;

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

            //string htmlBody = $"{friend.SecretSanta!.Name}";

            var secretSantaNameAsImage = DrawText(friend.SecretSanta!.Name, GetFont(FontFamily.GenericSansSerif), Color.Black, Color.FromArgb(188, 233, 226));

            AddImageToEmail(newMail, secretSantaNameAsImage);
            AddAttachment(newMail, friend.SecretSanta!.Name);

            //newMail.Body = htmlBody;

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

        private Font GetFont(FontFamily fontFamily)
        {
            return new Font(
               fontFamily,
               16,
               FontStyle.Regular,
               GraphicsUnit.Pixel);
        }

        private Image DrawText(string text, Font font, Color textColor, Color backColor)
        {
            //first, create a dummy bitmap just to get a graphics object
            Image img = new Bitmap(1, 1);
            Graphics drawing = Graphics.FromImage(img);

            //measure the string to see how big the image needs to be
            SizeF textSize = drawing.MeasureString(text, font);

            //free up the dummy image and old graphics object
            img.Dispose();
            drawing.Dispose();

            //create a new image of the right size
            img = new Bitmap((int)textSize.Width, (int)textSize.Height);

            drawing = Graphics.FromImage(img);

            //paint the background
            drawing.Clear(backColor);

            //create a brush for the text
            Brush textBrush = new SolidBrush(textColor);

            drawing.DrawString(text, font, textBrush, 0, 0);

            drawing.Save();

            textBrush.Dispose();
            drawing.Dispose();

            return img;
        }

        private static void AddAttachment(MailMessage mail, string friendName)
        {
            var attachment = Attachment.CreateAttachmentFromString($"<p>{friendName}</p>", MediaTypeNames.Text.Html);
            attachment.Name = $"{friendName}.html";
            mail.Attachments.Add(attachment);
        }

        private static void AddImageToEmail(MailMessage mail, Image image)
        {
            int value = Convert.ToInt32("2728", 16);
            var emoji = char.ConvertFromUtf32(value); //Sparklez

            var imageStream = GetImageStream(image);

            var imageResource = new LinkedResource(imageStream, "image/png") { ContentId = "attach1" };

            string body = string.Format(@"
                    <p>O seu amigo secreto é: </p>
                    <p> {1} </p> <img src=""cid:{0}"" /> <p> {2} </p>



                    <p>Obs: Se a imagem com o nome não aparecer, o amigo é o nome do anexo do email</p>
                ", imageResource.ContentId, emoji, emoji);

            var alternateView = AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html);

            alternateView.LinkedResources.Add(imageResource);
            mail.AlternateViews.Add(alternateView);
        }

        private static Stream GetImageStream(Image image)
        {
            // Conver the image to a memory stream and return.
            var imageConverter = new ImageConverter();
            var imgaBytes = (byte[])imageConverter.ConvertTo(image, typeof(byte[]));
            var memoryStream = new MemoryStream(imgaBytes);

            return memoryStream;
        }
    }
}

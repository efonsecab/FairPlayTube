using FairPlayTube.Services.Configuration;
using System.Net.Mail;
using System.Threading.Tasks;

namespace FairPlayTube.Services
{
    public class EmailService
    {
        private SmtpConfiguration SmtpConfiguration { get; }
        public EmailService(SmtpConfiguration smtpconfiguration)
        {
            this.SmtpConfiguration = smtpconfiguration;
        }

        public async Task SendEmail(string toEmailAddress, string subject, string body,
            bool isBodyHtml)
        {
            MailMessage msg = new MailMessage();
            msg.To.Add(new MailAddress(toEmailAddress));
            msg.From = new MailAddress(this.SmtpConfiguration.SenderEmail, this.SmtpConfiguration.SenderDisplayName);
            msg.Subject = subject;
            msg.Body = body;
            msg.IsBodyHtml = isBodyHtml;

            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(this.SmtpConfiguration.SenderUsername,
                this.SmtpConfiguration.SenderPassword);
            client.Port = this.SmtpConfiguration.Port;
            client.Host = this.SmtpConfiguration.Server;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            await client.SendMailAsync(msg).ConfigureAwait(continueOnCapturedContext: true);
        }
    }
}

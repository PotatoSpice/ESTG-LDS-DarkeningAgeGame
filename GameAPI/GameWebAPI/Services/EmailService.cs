using System;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace GameWebAPI.Services
{
    public interface IEmailService
    {
        void SendPasswordResetEmail(string to, string subject, string pwdResetToken, string origin);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            this._config = config;
        }

        public void SendPasswordResetEmail(string to, string subject, string pwdResetToken, string origin)
        {
            // create email message
            string message = "<h4>Reset Password Email</h4>";
            if (!string.IsNullOrEmpty(origin))
            {
                var resetUrl = $"{origin}/reset-password?token={pwdResetToken}";
                message += "<p>Please click the below link to reset your password, the link will be valid for 15 minutes:</p>"
                        + $"<p><a href=\"{resetUrl}\">{resetUrl}</a></p>";
            }
            else
            {
                message += $@"<p>Please use the below token to reset your password with the <code>*game-client-host*/reset-password?token=*token*</code> route:</p>
                             <p><code>{pwdResetToken}</code></p><p>The token is valid only for 15 minutes!</p>";
            }
            // create email
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(_config["AppSettings:EmailSettings:EmailFrom"]);
            mail.To.Add(to);
            mail.Subject = subject;
            mail.SubjectEncoding = System.Text.Encoding.UTF8;
            mail.Body = message;
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;

            // send email
            using (SmtpClient client = new SmtpClient())
            {
                client.Host = _config["AppSettings:EmailSettings:SmtpHost"];
                client.Port = int.Parse(_config["AppSettings:EmailSettings:SmtpPort"]);
                client.Credentials = new System.Net.NetworkCredential(
                    _config["AppSettings:EmailSettings:SmtpUser"], _config["AppSettings:EmailSettings:SmtpPwd"]);
                client.EnableSsl = true;

                client.Send(mail);
            }
        }
    }
}
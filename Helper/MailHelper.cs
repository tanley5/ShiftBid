using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace Shiftbid.Helper
{
    public class MailHelper
    {
        private IConfiguration configuration;
        public MailHelper(IConfiguration config)
        {
            configuration = config;
        }
        public bool Send(string from, string to, string subject, string content)
        {
            try
            {
                var host = configuration["Email:Host"];
                var port = int.Parse(configuration["Email:Port"]);
                var username = configuration["Email:Username"];
                var password = configuration["Email:Password"];
                var enable = bool.Parse(configuration["Email:SMTP:starttls:enable"]);

                var smtpClient = new SmtpClient
                {
                    Host = host,
                    Port = port,
                    EnableSsl = enable,
                    Credentials = new NetworkCredential(username, password)
                };

                var mailMessage = new MailMessage(from, to);
                mailMessage.Subject = subject;
                mailMessage.Body = content;
                mailMessage.IsBodyHtml = true;

                smtpClient.Send(mailMessage);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
    }
}
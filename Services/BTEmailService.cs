using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Threading.Tasks;
using TheBugTracker.Models;

namespace TheBugTracker.Services
{
    public class BTEmailService : IEmailSender
    {
        private readonly MailSettings _mailSettings;




        public BTEmailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }



        public async Task SendEmailAsync(string emailTo, string subject, string htmlMessage)
        {
            MimeMessage email = new();

            //added this
            //var emailSender = _mailSettings.Mail ?? Environment.GetEnvironmentVariable("Email");

            email.Sender = MailboxAddress.Parse(_mailSettings.Email);
            email.To.Add(MailboxAddress.Parse(emailTo));
            email.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = htmlMessage
            };

            email.Body = builder.ToMessageBody();

            try
            {
                using var smtp = new SmtpClient();

                //added this
                //var host = _mailSettings.Host ?? Environment.GetEnvironmentVariable("EmailHost");
                //var port = _mailSettings.Port != 0 ? _mailSettings.Port : int.Parse(Environment.GetEnvironmentVariable("EmailPort")!);
                //var password = _mailSettings.Password ?? Environment.GetEnvironmentVariable("EmailPassword");

                //swapped this
                //smtp.Connect(host, port, SecureSocketOptions.StartTls);
                //smtp.Authenticate(emailSender, password);

                smtp.Connect(_mailSettings.EmailHost, _mailSettings.EmailPort, SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSettings.Email, _mailSettings.EmailPassword);

                await smtp.SendAsync(email);
                smtp.Disconnect(true);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

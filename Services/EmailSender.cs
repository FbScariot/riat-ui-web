using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using RIAT.UI.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace RIAT.UI.Web.Services
{
    //public interface IEmailSender
    //{
    //    Task SendEmailAsync(string email, string subject, string message);
    //}

    public class EmailSender : IMyEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                // Credentials
                var credentials = new NetworkCredential(_emailSettings.Sender, _emailSettings.Password);

                // Mail message
                var mail = new MailMessage()
                {
                    From = new MailAddress(_emailSettings.Sender, _emailSettings.SenderName),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };

                mail.To.Add(new MailAddress(email));

                // Smtp client
                var client = new SmtpClient()
                {
                    Port = _emailSettings.MailPort,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = _emailSettings.MailServer,
                    EnableSsl = true,
                    Credentials = credentials
                };

                // Send it...         
                client.Send(mail);
            }
            catch (Exception ex)
            {
                // TODO: handle exception
                throw new InvalidOperationException(ex.Message);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="emails">email addresses must be separated with a comma character (",")</param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task SendEmailAsyncWithIcsAttachment(string emailsTO, string emailsBCC, string subject, string bodyMessage, string meeting)
        {
            try
            {
                // Credentials
                var credentials = new NetworkCredential(_emailSettings.Sender, _emailSettings.Password);
                
                var mail = new MailMessage()
                {
                    BodyEncoding = Encoding.UTF8,
                    From = new MailAddress(_emailSettings.Sender, _emailSettings.SenderName),
                    Subject = subject,
                    IsBodyHtml = true,
                };

                mail.To.Add(emailsTO);
                mail.Bcc.Add(emailsBCC);

                var htmlContentType = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Text.Html);
                var avHtmlBody = AlternateView.CreateAlternateViewFromString(bodyMessage, htmlContentType);
                mail.AlternateViews.Add(avHtmlBody);

                System.Net.Mime.ContentType ct = new System.Net.Mime.ContentType("text/calendar");
                ct.Parameters.Add("method", "REQUEST");
                ct.Parameters.Add("name", "meeting.ics");
                AlternateView avCalendar = AlternateView.CreateAlternateViewFromString(meeting, ct);
                mail.AlternateViews.Add(avCalendar);

                // Smtp client
                var client = new SmtpClient()
                {
                    Port = _emailSettings.MailPort,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = _emailSettings.MailServer,
                    EnableSsl = true,
                    Credentials = credentials
                };

                // Send it...         
                client.Send(mail);
            }
            catch (Exception ex)
            {
                // TODO: handle exception
                throw new InvalidOperationException(ex.Message);
            }

            return Task.CompletedTask;
        }
    }

    public interface IMyEmailSender : IEmailSender
    {
        Task SendEmailAsyncWithIcsAttachment(string emailsTO, string emailsBCC, string subject, string bodyMessage, string alternateView);
    }
}

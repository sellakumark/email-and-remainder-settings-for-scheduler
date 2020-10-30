using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace EmailScheduler.Data
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _mailConfig;
        private static string _mailResponse;

        public EmailService(EmailSettings mailConfig)
        {
            _mailConfig = mailConfig;
        }

        public async Task<string> SendEmailAsync(string ToEmailName, string Subject, string HTMLBody)
        {
            return await SendEmailAsync(new List<string>() { ToEmailName }, Subject, HTMLBody);

        }

        public async Task<string> SendEmailAsync(List<string> ToEmailName, string Subject, string HTMLBody)
        {
            _mailResponse = string.Empty;

            using (SmtpClient smtpClient = new SmtpClient(_mailConfig.Host, _mailConfig.Port))
            {
                smtpClient.UseDefaultCredentials = true;
                smtpClient.Credentials = new NetworkCredential(_mailConfig.Username, _mailConfig.Password);
                smtpClient.EnableSsl = true;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.SendCompleted += new SendCompletedEventHandler((object sender, AsyncCompletedEventArgs e) =>
                {
                    _mailResponse = (e.Error != null || e.Cancelled != false) ? "failure" : "success";
                });

                MailMessage message = new MailMessage();
                message.From = new MailAddress(_mailConfig.Username, _mailConfig.DisplayName);
                foreach (string EmailName in ToEmailName)
                {
                    message.To.Add(new MailAddress(EmailName));
                }
                message.Subject = Subject;
                message.SubjectEncoding = Encoding.UTF8;
                message.BodyEncoding = Encoding.UTF8;
                message.HeadersEncoding = Encoding.UTF8;
                message.IsBodyHtml = true;
                message.Body = HTMLBody;
                message.Priority = MailPriority.High;

                await smtpClient.SendMailAsync(message);
            }

            return _mailResponse;
        }

        public bool IsValidEmail(string EmailName)
        {
            return new EmailAddressAttribute().IsValid(EmailName);
        }

        public string GetEmailContent()
        {
            string HTMLBody = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\">\r\n" +
                "<html lang=\"en\" xml:lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\">\r\n\r\n" +
                "<head>\r\n" +
                "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=us-ascii\" />\r\n" +
                "<title>Emailing for Syncfusion Scheduler</title>\r\n" +
                "</head>\r\n\r\n" +
                "<body>\r\n" +
                "<table id=\"main-panel\" align=\"center\" width=\"750\" cellspacing=\"0\" cellpadding=\"0\" style=\"border: 1px solid #cccccc; color: #ffffff; margin-top: 10px;\">\r\n" +
                "<thead>\r\n" +
                "<tr height=\"75\">\r\n" +
                "<td class=\"##Notification##\" bgcolor=\"#6264a7\" style=\"padding: 0 30px;\">\r\n" +
                "<table cellspacing=\"0\" cellpadding=\"0\" width=\"100%\">\r\n" +
                "<tbody>\r\n" +
                "<tr>\r\n" +
                "<td style=\"padding:0;\">\r\n<b style=\"font-size: 20px;color: #ffffff;\">###EVENTTYPE###</b>\r\n</td>\r\n" +
                "</tr>\r\n" +
                "</tbody>\r\n" +
                "</table>\r\n" +
                "</td>\r\n" +
                "</tr>\r\n" +
                "</thead>\r\n" +
                "<tbody class=\"event-content\" style=\"background-color: #efefef80; color: #000000;\">\r\n" +
                "<tr>\r\n" +
                "<td style=\"padding: 0 30px;\">\r\n<div style=\"margin-top: 20px;\">Hi,</div>\r\n</td>\r\n" +
                "</tr>\r\n" +
                "<tr>\r\n" +
                "<td style=\"padding: 0 30px;\">\r\n<div style=\"margin-top: 15px;\">\r\nThe event has been scheduled to you and details has been listed below,\r\n</div>\r\n</td>\r\n" +
                "</tr>\r\n" +
                "<tr>\r\n<td style=\"height: 20px;\">&nbsp;</td>\r\n</tr>\r\n" +
                "<tr>\r\n" +
                "<td style=\"padding: 0 30px;\">\r\n" +
                "<table class=\"event-details\" cellspacing=\"0\" cellpadding=\"0\" width=\"100%\" style=\"border: 1px solid #ffffff;\">\r\n" +
                "<tbody>\r\n" +
                "<tr>\r\n" +
                "<td style=\"background-color: #efefef; border: 2px solid #ffffff; border-collapse: collapse; padding: 10px; vertical-align: top;\"><b>Subject</b></td>\r\n" +
                "<td style=\"background-color: #efefef; border: 2px solid #ffffff; border-collapse: collapse; padding: 10px; vertical-align: top;\">###SUBJECT###</td>\r\n" +
                "</tr>\r\n" +
                "<tr>\r\n" +
                "<td style=\"background-color: #efefef; border: 2px solid #ffffff; border-collapse: collapse; padding: 10px; vertical-align: top;\"><b>Start Time</b></td>\r\n" +
                "<td style=\"background-color: #efefef; border: 2px solid #ffffff; border-collapse: collapse; padding: 10px; vertical-align: top;\">###STARTTIME###</td>\r\n" +
                "</tr>\r\n" +
                "<tr>\r\n" +
                "<td style=\"background-color: #efefef; border: 2px solid #ffffff; border-collapse: collapse; padding: 10px; vertical-align: top;\"><b>End Time</b></td>\r\n" +
                "<td style=\"background-color: #efefef; border: 2px solid #ffffff; border-collapse: collapse; padding: 10px; vertical-align: top;\">###ENDTIME###</td>\r\n" +
                "</tr>\r\n" +
                "<tr>\r\n" +
                "<td style=\"background-color: #efefef; border: 2px solid #ffffff; border-collapse: collapse; padding: 10px; vertical-align: top;\"><b>Location</b></td>\r\n" +
                "<td style=\"background-color: #efefef; border: 2px solid #ffffff; border-collapse: collapse; padding: 10px; vertical-align: top;\">###LOCATION###</td>\r\n" +
                "</tr>\r\n" +
                "<tr>\r\n" +
                "<td style=\"background-color: #efefef; border: 2px solid #ffffff; border-collapse: collapse; padding: 10px; vertical-align: top;\"><b>Description</b></td>\r\n" +
                "<td style=\"background-color: #efefef; border: 2px solid #ffffff; border-collapse: collapse; padding: 10px; vertical-align: top;\">###DESCRIPTION###</td>\r\n" +
                "</tr>\r\n" +
                "</tbody>\r\n" +
                "</table>\r\n" +
                "</td>\r\n" +
                "</tr>\r\n" +
                "<tr>\r\n" +
                "<td style=\"height: 20px;\">&nbsp;</td>\r\n" +
                "</tr>\r\n" +
                "</tbody>\r\n" +
                "<tfoot>\r\n" +
                "<tr class=\"##Notification##\" bgcolor=\"#6264a7\" height=\"40\" style=\"color: white;\">\r\n" +
                "<td style=\"padding: 0 30px;\">Copyright © "+ DateTime.Now.Year.ToString() + " Syncfusion Scheduler</td>\r\n" +
                "</tr>\r\n" +
                "</tfoot>\r\n" +
                "</table>\r\n" +
                "</body>\r\n\r\n" +
                "</html>";
            return HTMLBody;
        }
    }

    public interface IEmailService
    {
        Task<string> SendEmailAsync(string ToEmailName, string Subject, string HTMLBody);
        Task<string> SendEmailAsync(List<string> ToEmailNames, string Subject, string HTMLBody);
        bool IsValidEmail(string EmailName);
        string GetEmailContent();
    }

    public class EmailSettings
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }

}

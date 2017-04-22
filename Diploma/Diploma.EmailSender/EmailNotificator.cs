using System.Net;
using System.Threading.Tasks;
using Diploma.EmailSender.Abstracts;
using Diploma.EmailSender.Models;
using MailKit.Net.Smtp;
using MimeKit;

namespace Diploma.EmailSender
{
    public class EmailNotificator : IEmailNotificator
    {
        private static string from = "diploma-support@yandex.ru";

        private MailboxAddress fromAddress = new MailboxAddress(from);

        private NetworkCredential networkCredential = new NetworkCredential(from, "123qweas");

        public async Task SendErrorReportToAdmin(ReportModel reportModel)
        {
            var emailMessage = CreateMessage(reportModel);

            emailMessage.To.Add(new MailboxAddress("opolkosergey@gmail.com"));

            await Send(emailMessage);
        }

        public async Task SendEventNotificationToUser(EventReportModel eventReportModel)
        {
            var emailMessage = CreateMessage(eventReportModel);

            emailMessage.To.Add(new MailboxAddress(eventReportModel.ToUser));

            await Send(emailMessage);
        }

        private MimeMessage CreateMessage(ReportModel reportModel)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(fromAddress);
            emailMessage.Subject = reportModel.Subject;
            emailMessage.Body = new TextPart("plain")
            {
                Text = reportModel.Body
            };

            return emailMessage;
        }

        private async Task Send(MimeMessage emailMessage)
        {
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.yandex.ru", 465, true);
                await client.AuthenticateAsync(networkCredential);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
} 
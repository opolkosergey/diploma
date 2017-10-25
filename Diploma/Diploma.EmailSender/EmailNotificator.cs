using System.Net;
using System.Threading.Tasks;
using Diploma.EmailSender.Abstracts;
using Diploma.EmailSender.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Diploma.EmailSender
{
    public class EmailNotificator : IEmailNotificator
    {
        private readonly EmailSenderOptions _emailSenderOptions;
        private readonly MailboxAddress _fromAddress;
        private readonly NetworkCredential _networkCredential;

        public EmailNotificator(IOptions<EmailSenderOptions> emailSenderOptions)
        {
            _emailSenderOptions = emailSenderOptions.Value;
            _fromAddress = new MailboxAddress(_emailSenderOptions.From);
            _networkCredential = new NetworkCredential(_emailSenderOptions.From, _emailSenderOptions.Password);
        }

        public async Task SendErrorReportToAdmin(ReportModel reportModel)
        {
            var emailMessage = CreateMessage(reportModel);

            emailMessage.To.Add(new MailboxAddress(_emailSenderOptions.To));

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

            emailMessage.From.Add(_fromAddress);
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
                await client.ConnectAsync(_emailSenderOptions.Host, _emailSenderOptions.Port, true);
                await client.AuthenticateAsync(_networkCredential);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
} 
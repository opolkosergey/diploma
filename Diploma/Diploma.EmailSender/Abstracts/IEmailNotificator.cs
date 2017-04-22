using System.Threading.Tasks;
using Diploma.EmailSender.Models;

namespace Diploma.EmailSender.Abstracts
{
    public interface IEmailNotificator
    {
        Task SendErrorReportToAdmin(ReportModel reportModel);

        Task SendEventNotificationToUser(EventReportModel eventReportModel);
    }
}

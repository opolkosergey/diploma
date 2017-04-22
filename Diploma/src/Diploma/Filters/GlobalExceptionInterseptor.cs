using Diploma.EmailSender.Abstracts;
using Diploma.EmailSender.Models;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Diploma.Filters
{
    public class GlobalExceptionInterseptor : IExceptionFilter
    {
        private readonly IEmailNotificator _emailNotificator;

        public GlobalExceptionInterseptor(IEmailNotificator _emailNotificator)
        {
            this._emailNotificator = _emailNotificator;
        }

        public async void OnException(ExceptionContext context)
        {
            await _emailNotificator.SendErrorReportToAdmin(new ReportModel
            {
                Subject = "System response exception.",
                Body = context.Exception.Message
            });
        }
    }
}

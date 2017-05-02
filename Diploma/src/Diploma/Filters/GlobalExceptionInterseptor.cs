using System;
using Diploma.Core.Models;
using Diploma.EmailSender.Abstracts;
using Diploma.EmailSender.Models;
using Diploma.Services.Abstracts;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Diploma.Filters
{
    public class GlobalExceptionInterseptor : IExceptionFilter
    {
        private readonly IEmailNotificator _emailNotificator;

        private readonly IAuditLogger _logger;

        public GlobalExceptionInterseptor(IEmailNotificator _emailNotificator, IAuditLogger logger)
        {
            this._emailNotificator = _emailNotificator;
            _logger = logger;
        }

        public async void OnException(ExceptionContext context)
        {
            await _logger.Log(new AuditEntry
            {
                DateTime = DateTime.Now,
                LogLevel = LogLevel.Error,
                Details = context.Exception.ToString()
            });

            await _emailNotificator.SendErrorReportToAdmin(new ReportModel
            {
                Subject = "System response exception.",
                Body = context.Exception.ToString()
            });
        }
    }
}

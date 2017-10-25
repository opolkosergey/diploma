using System;
using Diploma.Core.Models;
using Diploma.Core.Services;
using Diploma.EmailSender.Abstracts;
using Diploma.EmailSender.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Diploma.Filters
{
    public class GlobalExceptionInterseptor : IExceptionFilter
    {
        private readonly IEmailNotificator _emailNotificator;

        private readonly AuditLogger _logger;

        public GlobalExceptionInterseptor(IEmailNotificator emailNotificator, AuditLogger logger)
        {
            _emailNotificator = emailNotificator;
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

            //context.Result = new RedirectToActionResult("Error", "Home", null, true);
        }
    }
}

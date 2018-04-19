using System;
using Diploma.Core.Models;
using Diploma.Core.Services;
using Diploma.EmailSender.Abstracts;
using Diploma.EmailSender.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

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

        public void OnException(ExceptionContext context)
        {
            _logger.Log(new AuditEntry
            {
                DateTime = DateTime.Now,
                LogLevel = LogLevel.Error,
                Details = context.Exception.ToString()
            });

            _emailNotificator.SendErrorReportToAdmin(new ReportModel
            {
                Subject = "System response exception.",
                Body = context.Exception.ToString()
            });

            context.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                    { "controller", "Home" },
                    { "action", "Error" }
                });
            
            context.Result.ExecuteResultAsync(context);
        }
    }
}

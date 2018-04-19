using Diploma.Core.Services;
using Diploma.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Diploma.ViewModels;

namespace Diploma.Filters
{
    public class UserFoldersResultFilter : IResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext context)
        {
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            var viewResult = context.Result as ViewResult;
            if (viewResult != null)
            {
                if (string.IsNullOrEmpty(context.HttpContext.User.Identity.Name))
                {
                    viewResult.ViewData["Folders"] = FakeDataGenerator.GenerateFolders();
                }
                else
                {
                    var userService = context.HttpContext.RequestServices.GetRequiredService<UserService>();

                    var user = userService.GetUserByEmail(context.HttpContext.User.Identity.Name).GetAwaiter().GetResult();
                    viewResult.ViewData["Folders"] = user.UserFolders.Select(x => new LayoutUserFoldersModel
                    {
                        DocumentsCount = x.DocumentFolders.Count,
                        Id = x.Id,
                        Name = x.Name
                    }).ToList();
                }
            }
        }
    }
}

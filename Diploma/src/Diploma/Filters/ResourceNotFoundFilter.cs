using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace Diploma.Filters
{
    public class ResourceNotFoundFilter : IResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext context) {}

        public void OnResultExecuting(ResultExecutingContext context)
        {
            var result = context.Result as NotFoundObjectResult;

            if (result != null)
            {
                var message = result.Value.ToString();
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "controller", "Home" },
                        { "action", "Error" },
                        { "message", message },
                    });
                
                context.Result.ExecuteResultAsync(context);
            }
        }
    }
}

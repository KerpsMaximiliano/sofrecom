using Microsoft.AspNetCore.Mvc.Filters;
using Sofco.WebApi.Helpers;

namespace Sofco.WebApi.Filters
{
    public class VersionHeaderFilter : ActionFilterAttribute
    {
        public const string HeaderAppVersionName = "x-app-version";

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var response = context.HttpContext.Response;
            if (response == null)
                return;

            response.Headers.Add(HeaderAppVersionName, VersionHelper.Version);
        }
    }
}

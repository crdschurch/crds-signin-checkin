using System.Net;
using System.Web.Http.Filters;

namespace SignInCheckIn.ApiVersioning.Filters
{
    public class DeprecatedVersionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Request.Properties.ContainsKey("deprecated") && actionExecutedContext.Response.IsSuccessStatusCode)
            {
                actionExecutedContext.Response.StatusCode = (HttpStatusCode)299;
            }
            base.OnActionExecuted(actionExecutedContext);
        }
    }
}
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace SignInCheckIn.ApiVersioning.Filters
{
    public class RemovedVersionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionExecutingContext)
        {
            if (actionExecutingContext.Request.Properties.ContainsKey("removed"))
                actionExecutingContext.Request.CreateErrorResponse(HttpStatusCode.Gone, "The requested method has been removed");
            else
                base.OnActionExecuting(actionExecutingContext);
        }
    }
}
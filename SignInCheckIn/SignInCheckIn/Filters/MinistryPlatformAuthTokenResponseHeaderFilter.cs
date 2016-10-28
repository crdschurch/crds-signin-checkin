using System.Web.Http.Filters;

namespace SignInCheckIn.Filters
{
    public class MinistryPlatformAuthTokenResponseHeaderFilter : ActionFilterAttribute
    {
        public const string AuthorizationTokenHeaderName = "Crds-Mp-Auth-Token";
        public const string RefreshTokenHeaderName = "Crds-Mp-Refresh-Token";

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var authToken = actionExecutedContext.Request.Properties[AuthorizationTokenHeaderName];
            var refreshToken = actionExecutedContext.Request.Properties[AuthorizationTokenHeaderName];

            if (!string.IsNullOrEmpty(authToken?.ToString()))
            {
                actionExecutedContext.Response.Content.Headers.Add(AuthorizationTokenHeaderName, authToken.ToString());
            }

            if (!string.IsNullOrEmpty(refreshToken?.ToString()))
            {
                actionExecutedContext.Response.Content.Headers.Add(RefreshTokenHeaderName, refreshToken.ToString());
            }
        }
    }
}
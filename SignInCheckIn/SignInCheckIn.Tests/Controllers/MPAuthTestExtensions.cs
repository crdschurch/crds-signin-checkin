using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http.Controllers;
using crds_angular.Security;

namespace SignInCheckIn.Tests.Controllers
{
    // ReSharper disable once InconsistentNaming
    public static class MPAuthTestExtensions
    {
        public const string AuthType = "Bearer";
        public const string AuthToken = "tok123";

        public static void SetupAuthorization(this MPAuth controller, string authType, string authToken)
        {
            controller.Request = new HttpRequestMessage();
            controller.Request.Headers.Authorization = new AuthenticationHeaderValue(authType, authToken);
            controller.RequestContext = new HttpRequestContext();
        }
    }
}

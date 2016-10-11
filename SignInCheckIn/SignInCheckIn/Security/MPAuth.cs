using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using System.Web.Http.Description;
using System.Web.SessionState;
using System.Diagnostics;
using log4net;
using log4net.Config;
using System.Reflection;
using System.Threading;
//using crds_angular.Models.Crossroads;
//using crds_angular.Util;
//using Microsoft.Ajax.Utilities;
//using Microsoft.Owin;
using MinistryPlatform.Translation.Repositories;

namespace crds_angular.Security
{
    public class MPAuth : ApiController
    {
        protected readonly log4net.ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Ensure that a user is authenticated before executing the given lambda expression.  The expression will
        /// have a reference to the user's authentication token (the value of the "Authorization" cookie).  If
        /// the user is not authenticated, an UnauthorizedResult will be returned.
        /// </summary>
        /// <param name="doIt">A lambda expression to execute if the user is authenticated</param>
        /// <returns>An IHttpActionResult from the "doIt" expression, or UnauthorizedResult if the user is not authenticated.</returns>
        protected IHttpActionResult Authorized(Func<string, IHttpActionResult> doIt)
        {
            return (Authorized(doIt, () => { return (Unauthorized()); }));
        }

        /// <summary>
        /// Execute the lambda expression "actionWhenAuthorized" if the user is authenticated, or execute the expression
        /// "actionWhenNotAuthorized" if the user is not authenticated.  If authenticated, the "actionWhenAuthorized"
        /// expression will have a reference to the user's authentication token (the value of the "Authorization" cookie).
        /// </summary>
        /// <param name="actionWhenAuthorized">A lambda expression to execute if the user is authenticated</param>
        /// <param name="actionWhenNotAuthorized">A lambda expression to execute if the user is NOT authenticated</param>
        /// <returns>An IHttpActionResult from the lambda expression that was executed.</returns>
        protected IHttpActionResult Authorized(Func<string, IHttpActionResult> actionWhenAuthorized, Func<IHttpActionResult> actionWhenNotAuthorized)
        {
            //try
            //{
            //    IEnumerable<string> refreshTokens;
            //    var authorized = "";
            //    if (Request.Headers.TryGetValues("RefreshToken", out refreshTokens) && refreshTokens.Any())
            //    {
            //        var authData = AuthenticationRepository.RefreshToken(refreshTokens.FirstOrDefault());
            //        if (authData != null)
            //        {
            //            authorized = authData["token"].ToString();
            //            var RefreshToken = authData["RefreshToken"].ToString();
            //            var result = new HttpAuthResult(actionWhenAuthorized(authorized), authorized, RefreshToken);
            //            return result;
            //        }
            //    }

            //    authorized = Request.Headers.GetValues("Authorization").FirstOrDefault();
            //    if (authorized != null && (authorized != "null" || authorized != ""))
            //    {
            //        return actionWhenAuthorized(authorized);
            //    }
            //    return actionWhenNotAuthorized();
            //}
            //catch (System.InvalidOperationException)
            //{
            //    return actionWhenNotAuthorized();
            //}

            return null;
        }
    }
}
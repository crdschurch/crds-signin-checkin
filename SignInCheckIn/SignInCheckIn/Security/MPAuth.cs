using System;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using Crossroads.Web.Common.Security;
using log4net;
using SignInCheckIn.Util;
using System.Collections.Generic;
using Crossroads.Web.Common.Services;
using System.Net.Http.Headers;

namespace SignInCheckIn.Security
{
    public class MpAuth : ApiController
    {
        private readonly IAuthTokenExpiryService _authTokenExpiryService;
        protected readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IAuthenticationRepository _authenticationRepository;

        public MpAuth(IAuthTokenExpiryService authTokenExpiryService, IAuthenticationRepository authenticationRepository)
        {
            _authTokenExpiryService = authTokenExpiryService;
            _authenticationRepository = authenticationRepository;
        }

        /// <summary>
        /// Ensure that a user is authenticated before executing the given lambda expression.  The expression will
        /// have a reference to the user's authentication token (the value of the "Authorization" cookie).  If
        /// the user is not authenticated, an UnauthorizedResult will be returned.
        /// </summary>
        /// <param name="doIt">A lambda expression to execute if the user is authenticated</param>
        /// <returns>An IHttpActionResult from the "doIt" expression, or UnauthorizedResult if the user is not authenticated.</returns>
        protected IHttpActionResult Authorized(Func<string, IHttpActionResult> doIt)
        {
            return Authorized(doIt, () => Unauthorized());
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
            try
            {
                IEnumerable<string> refreshTokens;
                var authorized = "";

                bool authTokenCloseToExpiry = _authTokenExpiryService.IsAuthTokenCloseToExpiry(Request.Headers);
                bool isRefreshTokenPresent =
                    Request.Headers.TryGetValues("RefreshToken", out refreshTokens) && refreshTokens.Any();

                HttpRequestHeaders headers = Request.Headers;

                if (authTokenCloseToExpiry && isRefreshTokenPresent)
                {
                    var authData = _authenticationRepository.RefreshToken(refreshTokens.FirstOrDefault());
                    if (authData != null)
                    {
                        authorized = authData.AccessToken;
                        var refreshToken = authData.RefreshToken;
                        return new HttpAuthResult(actionWhenAuthorized(authorized), authorized, refreshToken);
                    }
                }

                authorized = Request.Headers.GetValues("Authorization").FirstOrDefault();
                if (authorized != null && (authorized != "null" || authorized != ""))
                {
                    return actionWhenAuthorized(authorized);
                }
                return actionWhenNotAuthorized();
            }
            catch (System.InvalidOperationException e)
            {
                return actionWhenNotAuthorized();
            }
        }
    }
}
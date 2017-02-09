using System;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using log4net;
using Microsoft.AspNet.SignalR;
using MinistryPlatform.Translation.Repositories.Interfaces;
using SignInCheckIn.Hubs;
using SignInCheckIn.Util;

namespace SignInCheckIn.Security
{
    public class MpAuth : ApiController
    {
        protected readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IAuthenticationRepository _authenticationRepository;

        public MpAuth(IAuthenticationRepository authenticationRepository)
        {
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
                var refreshTokenHeader = Request.Headers.Contains(HttpAuthResult.RefreshTokenHeaderName)
                    ? Request.Headers.GetValues(HttpAuthResult.RefreshTokenHeaderName).FirstOrDefault()
                    : null;
                if (refreshTokenHeader != null)
                {
                    var authData = _authenticationRepository.RefreshToken(refreshTokenHeader);
                    if (authData != null)
                    {
                        var authToken = authData["token"].ToString();
                        var refreshToken = authData["refreshToken"].ToString();
                        var result = new HttpAuthResult(actionWhenAuthorized(authToken), authToken, refreshToken);
                        return result;
                    }
                }

                var authorized = Request.Headers.Contains(HttpAuthResult.AuthorizationTokenHeaderName)
                    ? Request.Headers.GetValues(HttpAuthResult.AuthorizationTokenHeaderName).FirstOrDefault()
                    : null;
                if (!string.IsNullOrEmpty(authorized) && !authorized.Equals("null"))
                {
                    return actionWhenAuthorized(authorized);
                }
                return actionWhenNotAuthorized();
            }
            catch (InvalidOperationException)
            {
                return actionWhenNotAuthorized();
            }
        }

        protected static void PublishToChannel(IHubContext context, ChannelEvent channelEvent)
        {
            // Taken from: https://github.com/sstorie/experiments/tree/master/angular2-signalr
            // From .NET code like this we can't invoke the methods that
            //  exist on our actual Hub class...because we only have a proxy
            //  to it. So to publish the event we need to call the method that
            //  the clients will be listening on.
            context.Clients.Group(channelEvent.ChannelName).OnEvent(channelEvent.ChannelName, channelEvent);
        }
    }
}
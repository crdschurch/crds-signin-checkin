using System;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Description;
using log4net;
using SignInCheckIn.Filters;
using SignInCheckIn.Models.Authentication;
using SignInCheckIn.Services.Interfaces;
using SignInCheckIn.Util;

namespace SignInCheckIn.Controllers
{
    public class LoginController : ApiController
    {
        private readonly ILoginService _loginService;
        protected readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost]
        [ResponseType(typeof(LoginReturn))]
        [Route("authenticate")]
        public IHttpActionResult Authenticate([FromBody] Credentials cred)
        {
            try
            {
                var loginReturn = _loginService.Login(cred.Username, cred.Password);
                return new HttpAuthResult(Ok(loginReturn), loginReturn.UserToken, loginReturn.RefreshToken);
            }
            catch (Exception e)
            {
                if (e.GetType().Name == "UnauthorizedAccessException")
                {
                    return Unauthorized();
                }

                Logger.Error("Error authenticating user", e);

                return InternalServerError();                
            }
        }
    }
}

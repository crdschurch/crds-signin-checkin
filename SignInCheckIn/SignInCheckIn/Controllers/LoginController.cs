using System;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Description;
using log4net;
using SignInCheckIn.Models.Authentication;
using SignInCheckIn.Services.Interfaces;

namespace SignInCheckIn.Controllers
{
    public class LoginController : ApiController
    {
        private readonly ILoginService _loginService;
        protected readonly log4net.ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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

                return this.Ok(loginReturn);
            }
            catch (Exception e)
            {
                if (e.GetType().Name == "UnauthorizedAccessException")
                {
                    return Unauthorized();
                }

                logger.Error("Error authenticating user", e);

                return InternalServerError();                
            }
        }
    }
}

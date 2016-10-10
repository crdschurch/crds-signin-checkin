using System;
using System.Web.Http;
using System.Web.Http.Description;
using SignInCheckIn.Models.Authentication;
using SignInCheckIn.Services.Interfaces;

namespace SignInCheckIn.Controllers
{
    public class LoginController : ApiController
    {
        private readonly ILoginService _loginService;

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
                //var apiError = new ApiErrorDto("Login Failed", e);
                //throw new HttpResponseException(apiError.HttpResponseMessage);
                return InternalServerError(e);
            }
        }
    }
}

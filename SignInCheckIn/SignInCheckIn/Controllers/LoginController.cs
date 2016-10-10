using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using MinistryPlatform.Translation.Models.DTO;
using SignInCheckIn.Models.Authentication;
using SignInCheckIn.Services;
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

        //[ResponseType(typeof(LoginReturn))]
        //public IHttpActionResult Post([FromBody] Credentials cred)
        [HttpPost]
        [ResponseType(typeof(LoginReturn))]
        [Route("authenticate")]
        public IHttpActionResult Authenticate([FromBody] Credentials cred)
        {
            try
            {
                // try to login 
                //var authData = _loginService.Login(cred.username, cred.password);


                var loginReturn = _loginService.Login(cred.username, cred.password);

                /*** Dustin, add whatever role you need here. Thanks, John. ***/
                //loginReturn.roles.Add(new MpRoleDto
                //{
                    
                //});

                //var token = authData["token"].ToString();
                //var exp = authData["exp"].ToString();
                //var refreshToken = authData["refreshToken"].ToString();

                //if (token == "")
                //{
                //    return this.Unauthorized();
                //}

                //var userRoles = _personService.GetLoggedInUserRoles(token);
                //var p = _personService.GetLoggedInUserProfile(token);
                //var r = new LoginReturn
                //{
                //    userToken = token,
                //    userTokenExp = exp,
                //    refreshToken = refreshToken,
                //    userId = p.ContactId,
                //    username = p.FirstName,
                //    userEmail = p.EmailAddress,
                //    roles = userRoles,
                //    age = p.Age
                //};

                //_loginService.ClearResetToken(cred.username);

                //HttpResponseHeadersExtensions.AddCookies();

                //return this.Ok(r);
                return this.Ok(loginReturn);
            }
            catch (Exception e)
            {
                //var apiError = new ApiErrorDto("Login Failed", e);
                //throw new HttpResponseException(apiError.HttpResponseMessage);
                return InternalServerError(e);
            }

            //return Ok();
        }

        //public class LoginReturn
        //{
        //    public LoginReturn() { }
        //    public LoginReturn(string userToken, int userId, string username, string userEmail, List<MpRoleDto> roles)
        //    {
        //        this.userId = userId;
        //        this.userToken = userToken;
        //        this.username = username;
        //        this.userEmail = userEmail;
        //        this.roles = roles;
        //    }
        //    public string userToken { get; set; }
        //    public string userTokenExp { get; set; }
        //    public string refreshToken { get; set; }
        //    public int userId { get; set; }
        //    public string username { get; set; }
        //    public string userEmail { get; set; }
        //    public List<MpRoleDto> roles { get; set; }
        //    public int age { get; set; }
        //}

        public class Credentials
        {
            public string username { get; set; }
            public string password { get; set; }
        }
    }
}

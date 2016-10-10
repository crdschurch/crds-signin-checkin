using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using MinistryPlatform.Translation.Models.DTO;
using SignInCheckIn.Services;

namespace SignInCheckIn.Controllers
{
    public class LoginController : ApiController
    {
        [ResponseType(typeof(LoginReturn))]
        public IHttpActionResult Post([FromBody] Credentials cred)
        {
            //try
            //{
            //    // try to login 
            //    var authData = LoginService.Login(cred.username, cred.password);
            //    var token = authData["token"].ToString();
            //    var exp = authData["exp"].ToString();
            //    var refreshToken = authData["refreshToken"].ToString();

            //    if (token == "")
            //    {
            //        return this.Unauthorized();
            //    }

            //    var userRoles = _personService.GetLoggedInUserRoles(token);
            //    var p = _personService.GetLoggedInUserProfile(token);
            //    var r = new LoginReturn
            //    {
            //        userToken = token,
            //        userTokenExp = exp,
            //        refreshToken = refreshToken,
            //        userId = p.ContactId,
            //        username = p.FirstName,
            //        userEmail = p.EmailAddress,
            //        roles = userRoles,
            //        age = p.Age
            //    };

            //    _loginService.ClearResetToken(cred.username);

            //    //ttpResponseHeadersExtensions.AddCookies();

            //    return this.Ok(r);
            //}
            //catch (Exception e)
            //{
            //    var apiError = new ApiErrorDto("Login Failed", e);
            //    throw new HttpResponseException(apiError.HttpResponseMessage);
            //}

            return Ok();
        }

        public class LoginReturn
        {
            public LoginReturn() { }
            public LoginReturn(string userToken, int userId, string username, string userEmail, List<MpRoleDto> roles)
            {
                this.userId = userId;
                this.userToken = userToken;
                this.username = username;
                this.userEmail = userEmail;
                this.roles = roles;
            }
            public string userToken { get; set; }
            public string userTokenExp { get; set; }
            public string refreshToken { get; set; }
            public int userId { get; set; }
            public string username { get; set; }
            public string userEmail { get; set; }
            public List<MpRoleDto> roles { get; set; }
            public int age { get; set; }
        }

        public class Credentials
        {
            public string username { get; set; }
            public string password { get; set; }
        }
    }
}

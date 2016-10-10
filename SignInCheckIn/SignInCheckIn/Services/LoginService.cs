using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MinistryPlatform.Translation.Repositories.Interfaces;
using SignInCheckIn.Models.Authentication;
using SignInCheckIn.Services.Interfaces;

namespace SignInCheckIn.Services
{
    public class LoginService : ILoginService
    {
        private IAuthenticationRepository _authenticationRepository;

        public LoginService(IAuthenticationRepository authenticationRepository)
        {
            _authenticationRepository = authenticationRepository;
        }

        public LoginReturn Login(string username, string password)
        {
            var authData = _authenticationRepository.Authenticate(username, password);

            LoginReturn loginReturn = new LoginReturn
            {
                userToken = authData["token"].ToString(),
                userTokenExp = authData["exp"].ToString(),
                refreshToken = authData["refreshToken"].ToString()
            };

            return loginReturn;
        }
    }
}
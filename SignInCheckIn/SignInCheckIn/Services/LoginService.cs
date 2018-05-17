using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Crossroads.Web.Common.Security;
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
            var authData = _authenticationRepository.AuthenticateUser(username, password, true);

            if (authData == null)
            {
                throw new UnauthorizedAccessException();
            }

            LoginReturn loginReturn = new LoginReturn
            {
                UserToken = authData.AccessToken,
                UserTokenExp = authData.ExpiresIn + "",
                RefreshToken = authData.RefreshToken
            };

            var roles = _authenticationRepository.GetUserRolesFromToken(loginReturn.UserToken);

            if (!roles.Contains("Kids Club Tools - CRDS"))
            {
                throw new UnauthorizedAccessException();
            }

            loginReturn.Roles = roles;

            return loginReturn;
        }
    }
}
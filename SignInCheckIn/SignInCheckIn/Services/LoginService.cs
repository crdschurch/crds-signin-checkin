using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MinistryPlatform.Translation.Repositories.Interfaces;
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

        public void Login(string username, string password)
        {
            _authenticationRepository.Authenticate(username, password);
            var x = 1;
        }
    }
}
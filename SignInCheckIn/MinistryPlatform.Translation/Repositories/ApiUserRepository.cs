using Crossroads.Utilities.Interfaces;
using MinistryPlatform.Translation.Repositories.Interfaces;

namespace MinistryPlatform.Translation.Repositories
{
    public class ApiUserRepository : IApiUserRepository
    {
        private readonly IConfigurationWrapper _configurationWrapper;
        private readonly IAuthenticationRepository _authenticationService;

        public ApiUserRepository(IConfigurationWrapper configurationWrapper, IAuthenticationRepository authenticationService)
        {
            _configurationWrapper = configurationWrapper;
            _authenticationService = authenticationService;
        }

        public string GetToken()
        {
            var apiUser = _configurationWrapper.GetEnvironmentVarAsString("API_USER");
            var apiPasword = _configurationWrapper.GetEnvironmentVarAsString("API_PASSWORD");
            var authData = _authenticationService.Authenticate(apiUser, apiPasword);
            var token = authData["token"].ToString();

            return (token);
        }
    }
}
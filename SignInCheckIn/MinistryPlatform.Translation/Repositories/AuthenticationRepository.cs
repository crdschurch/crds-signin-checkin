using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using Crossroads.Utilities.Interfaces;
using MinistryPlatform.Translation.Repositories.Interfaces;
//using System.Net.Http;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;

namespace MinistryPlatform.Translation.Repositories
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        //private readonly IConfigurationWrapper _configurationWrapper;

        //public AuthenticationRepository(IConfigurationWrapper configurationWrapper)
        //{
        //    _configurationWrapper = configurationWrapper;
        //}

        public Dictionary<string, object> Authenticate(string username, string password)
        {
            var userCredentials =
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"username", username},
                    {"password", password},
                    {"client_id", "client"},
                    {"client_secret", "secret"},
                    {"grant_type", "password"}
                });
    
            HttpClient client = new HttpClient();
            var tokenUrl = ConfigurationManager.AppSettings["TokenURL"];
            var message = client.PostAsync(tokenUrl, userCredentials);
            try
            {
                var result = message.Result.Content.ReadAsStringAsync().Result;
                var obj = JObject.Parse(result);
                var token = (string)obj["access_token"];
                var exp = (string)obj["expires_in"];
                var refreshToken = (string)obj["refresh_token"];
                var authData = new Dictionary<string, object>
                {
                    {"token", token},
                    {"exp", exp},
                    {"refreshToken", refreshToken}
                };
                return authData;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public Dictionary<string, object> RefreshToken(string refreshToken)
        {
            var userCredentials =
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"refresh_token", refreshToken},
                    {"client_id", "client"},
                    {"client_secret", "secret"},
                    {"grant_type", "refresh_token"}
                });

            HttpClient client = new HttpClient();
            var tokenUrl = ConfigurationManager.AppSettings["TokenURL"];
            var message = client.PostAsync(tokenUrl, userCredentials);
            try
            {
                var result = message.Result.Content.ReadAsStringAsync().Result;
                var obj = JObject.Parse(result);
                var token = (string)obj["access_token"];
                var exp = (string)obj["expires_in"];
                var refreshTokenResponse = (string)obj["refresh_token"];
                var authData = new Dictionary<string, object>
                {
                    {"token", token},
                    {"exp", exp},
                    {"refreshToken", refreshTokenResponse}
                };
                return authData;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public Dictionary<string, object> GetUserFromToken(string userToken)
        {
            userToken =
                "AAEAALIi5nTOzPzesOdEEdK16VmmTYt2ayCMbhp3g7qXHzOwyxmyu_27K1XETWuZjOfiNj7uWIvK4VbzefcTmTeo8ulxB7rD-_-7kCDbDV-vLsi32yOkJP9vjRQQ73su7WFcjyS2v0aKAnxdnrtZBBvJKzQNEZcjQQNWQ7ZgPX32ug9e6ZT1DOsUQE4jp2DutMZbyckOElS6Pu-ex87cuqNXuqTzzb1PsU6x6mcS-ZtImi8bHFLR6UY8m1TaL2Jsbs9O5n_uiRgncJXiV3YP2wnROoorVQjJwyujpKX1nZKtDn8qtryEs2InxulN6xxDl9vUIJiuoqyRbC6kZPCqDTk9hpfIAAAATGlmZXRpbWU9MTgwMCZDbGllbnRJZGVudGlmaWVyPWNsaWVudCZVc2VyPTNkZjU1NjNhLWJkMzEtNGViMy05OGEzLTBlNTM4ZDU2ZDdhMCZTY29wZT1odHRwJTNBJTJGJTJGd3d3LnRoaW5rbWluaXN0cnkuY29tJTJGZGF0YXBsYXRmb3JtJTJGc2NvcGVzJTJGYWxsJnRzPTE0NzU4NjcyMjgmdD1Eb3ROZXRPcGVuQXV0aC5PQXV0aDIuQWNjZXNzVG9rZW4";

            var userCredentials =
                new FormUrlEncodedContent(new Dictionary<string, string>
                {

                });

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + userToken);
            
            var tokenUrl = ConfigurationManager.AppSettings["TokenURL"];
            var message = client.PostAsync(tokenUrl, userCredentials);
            try
            {
                var result = message.Result.Content.ReadAsStringAsync().Result;
                var obj = JObject.Parse(result);
                var token = (string)obj["access_token"];
                var exp = (string)obj["expires_in"];
                var refreshTokenResponse = (string)obj["refresh_token"];
                var authData = new Dictionary<string, object>
                {
                    {"token", token},
                    {"exp", exp},
                    {"refreshToken", refreshTokenResponse}
                };
                return authData;
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}

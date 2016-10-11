using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Newtonsoft.Json.Linq;

namespace MinistryPlatform.Translation.Repositories
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
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
        public List<string> GetUserRolesFromToken(string userToken)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + userToken);

            var userCredentials =
                new FormUrlEncodedContent(new Dictionary<string, string>
                {

                });

            var tokenUrl = ConfigurationManager.AppSettings["TokenURL"];
            var message = client.PostAsync(tokenUrl, userCredentials);
            try
            {
                var result = message.Result.Content.ReadAsStringAsync().Result;
                var obj = JObject.Parse(result);
                var roles = JArray.Parse(obj["roles"].ToString());

                var rolesInfo = roles.ToObject<List<string>>();

                return rolesInfo;
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}

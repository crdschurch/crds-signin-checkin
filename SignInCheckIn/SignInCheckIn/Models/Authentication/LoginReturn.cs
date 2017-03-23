using System.Collections.Generic;
using Newtonsoft.Json;

namespace SignInCheckIn.Models.Authentication
{
    public class LoginReturn
    {
        [JsonProperty("userToken")]
        public string UserToken { get; set; }
        [JsonProperty("userTokenExp")]
        public string UserTokenExp { get; set; }
        [JsonProperty("refreshToken")]
        public string RefreshToken { get; set; }
        [JsonProperty("roles")]
        public List<string> Roles { get; set; }
    }
}
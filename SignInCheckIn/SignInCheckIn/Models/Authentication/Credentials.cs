using Newtonsoft.Json;

namespace SignInCheckIn.Models.Authentication
{
    public class Credentials
    {
        [JsonProperty("Username")]
        public string Username { get; set; }
        [JsonProperty("Password")]
        public string Password { get; set; }
    }
}
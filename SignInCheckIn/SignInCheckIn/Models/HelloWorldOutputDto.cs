using Newtonsoft.Json;

namespace SignInCheckIn.Models
{
    public class HelloWorldOutputDto
    {
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("name")]
        public virtual string Name { get; set; }
    }
}
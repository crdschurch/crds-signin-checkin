using System;
using Newtonsoft.Json;

namespace SignInCheckIn.Models
{
    public class HelloWorldV2OutputDto : HelloWorldOutputDto
    {
        [JsonIgnore, Obsolete("name is deprecated", true)]
        public override string Name { get; set; }
        [JsonProperty("firstName")]
        public string FirstName { get; set; }
        [JsonProperty("lastName")]
        public string LastName { get; set; }
    }
}
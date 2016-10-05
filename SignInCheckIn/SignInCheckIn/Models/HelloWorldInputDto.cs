using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SignInCheckIn.Models
{
    public class HelloWorldInputDto
    {
        [JsonProperty("name")]
        public virtual string Name { get; set; }
    }
}
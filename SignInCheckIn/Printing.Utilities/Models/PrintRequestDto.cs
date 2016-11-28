using Newtonsoft.Json;

namespace Printing.Utilities.Models
{
    public class PrintRequestDto
    {
        [JsonProperty(PropertyName = "printerId")]
        public int printerId { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string title { get; set; } // should be descriptive of origin and purpose

        [JsonProperty(PropertyName = "contentType")]
        public string contentType { get; set; }

        [JsonProperty(PropertyName = "content")]
        public string content { get; set; }

        [JsonProperty(PropertyName = "source")]
        public string source { get; set; }
    }
}

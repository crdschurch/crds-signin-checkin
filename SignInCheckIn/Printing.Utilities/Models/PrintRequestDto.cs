using Newtonsoft.Json;

namespace Printing.Utilities.Models
{
    public class PrintRequestDto
    {
        [JsonProperty(PropertyName = "printerId")]
        public int PrinterId { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; } // should be descriptive of origin and purpose

        [JsonProperty(PropertyName = "contentType")]
        public string ContentType { get; set; }

        [JsonProperty(PropertyName = "content")]
        public string Content { get; set; }
    }
}

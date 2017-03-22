using Newtonsoft.Json;

namespace MinistryPlatform.Translation.Models.DTO
{
    public class MpCongregationDto
    {
        [JsonProperty(PropertyName = "Congregation_ID")]
        public int CongregationId { get; set; }

        [JsonProperty(PropertyName = "Congregation_Name")]
        public string CongregationName { get; set; }
    }
}

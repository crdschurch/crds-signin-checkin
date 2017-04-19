using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;

namespace MinistryPlatform.Translation.Models.DTO
{
    [MpRestApiTable(Name = "Countries")]
    public class MpCountryDto
    {
        [JsonProperty(PropertyName = "Country_ID")]
        public int CountryId { get; set; }

        [JsonProperty(PropertyName = "Country")]
        public string Country { get; set; }

        [JsonProperty(PropertyName = "Code3")]
        public string Code3 { get; set; }

    }
}

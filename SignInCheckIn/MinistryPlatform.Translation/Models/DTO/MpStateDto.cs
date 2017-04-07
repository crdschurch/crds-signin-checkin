using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;

namespace MinistryPlatform.Translation.Models.DTO
{
    [MpRestApiTable(Name = "States")]
    public class MpStateDto
    {
        [JsonProperty(PropertyName = "State_ID")]
        public int StateId { get; set; }

        [JsonProperty(PropertyName = "State_Name")]
        public string StateName { get; set; }

        [JsonProperty(PropertyName = "State_Abbreviation")]
        public string StateAbbreviation { get; set; }
    }
}

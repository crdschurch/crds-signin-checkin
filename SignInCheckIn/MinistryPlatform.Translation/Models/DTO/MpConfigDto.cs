using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;

namespace MinistryPlatform.Translation.Models.DTO
{
    [MpRestApiTable(Name = "dp_Configuration_Settings")]
    public class MpConfigDto
    {
        [JsonProperty(PropertyName = "Configuration_Setting_ID")]
        public int ConfigurationSettingId { get; set; }

        [JsonProperty(PropertyName = "Application_Code")]
        public string ApplicationCode { get; set; }

        [JsonProperty(PropertyName = "Key_Name")]
        public string KeyName { get; set; }

        [JsonProperty(PropertyName = "Value")]
        public string Value { get; set; }

        [JsonProperty(PropertyName = "Description")]
        public string Description { get; set; }
    }
}

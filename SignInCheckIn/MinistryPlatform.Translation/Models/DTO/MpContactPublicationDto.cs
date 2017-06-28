using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;

namespace MinistryPlatform.Translation.Models.DTO
{
    [MpRestApiTable(Name = "dp_Contact_Publications")]
    public class MpContactPublicationDto
    {
        [JsonProperty(PropertyName = "Contact_ID")]
        public int ContactId { get; set; }

        [JsonProperty(PropertyName = "Publication_ID")]
        public int PublicationId { get; set; }

        [JsonProperty(PropertyName = "Unsubscribed")]
        public bool Unsubscribed { get; set; }
    }
}

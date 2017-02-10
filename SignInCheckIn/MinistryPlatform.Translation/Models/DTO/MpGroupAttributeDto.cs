using MinistryPlatform.Translation.Models.Attributes;
using Newtonsoft.Json;

namespace MinistryPlatform.Translation.Models.DTO
{
    [MpRestApiTable(Name = "Group_Attributes")]
    public class MpGroupAttributeDto
    {
        [JsonProperty("Group_ID")]
        public int GroupId { get; set; }

        [JsonProperty("Attribute_Type_ID")]
        public int AttributeTypeId { get; set; }

        [JsonProperty("Attribute_ID")]
        public int AttributeId { get; set; }

        [JsonProperty("Attribute_Name")]
        public string AttributeName { get; set; }

        [JsonProperty("Sort_Order")]
        public int SortOrder { get; set; }

        public MpAttributeDto GetAttributeDto()
        {
            return new MpAttributeDto
            {
                Id = AttributeId,
                Name = AttributeName,
                SortOrder = SortOrder
            };
        }
    }
}

using System;
using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;

namespace MinistryPlatform.Translation.Models.DTO
{
    [MpRestApiTable(Name = "Attribute_Types")]
    public class MpAttributeTypeDto
    {
        [JsonProperty("Attribute_Type_ID")]
        public int Id { get; set; }
        [JsonProperty("Attribute_Type")]
        public string Name { get; set; }
    }
}

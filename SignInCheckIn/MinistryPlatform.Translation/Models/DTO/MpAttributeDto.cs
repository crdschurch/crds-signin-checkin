using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Translation.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MinistryPlatform.Translation.Models.DTO
{
    [MpRestApiTable(Name = "Attributes")]
    public class MpAttributeDto
    {
        [JsonExtensionData]
#pragma warning disable 649
        private IDictionary<string, JToken> _unmappedData;
#pragma warning restore 649
        [JsonIgnore]
        public MpAttributeTypeDto Type { get; set; }
        [JsonProperty("Attribute_ID")]
        public int Id { get; set; }
        [JsonProperty("Attribute_Name")]
        public string Name { get; set; }
        [JsonProperty("Sort_Order")]
        public int SortOrder { get; set; }
        [JsonProperty(PropertyName = "Attribute_Category")]
        public string Category { get; set; }
        [JsonProperty(PropertyName = "Attribute_Category_ID")]
        public int? CategoryId { get; set; }
        [JsonProperty(PropertyName = "Attribute_Category_Description")]
        public string CategoryDescription { get; set; }
        public bool? Selected { get; set; }
        [JsonProperty(PropertyName = "Attribute_Type_ID")]
        public int AttributeTypeId { get; set; }
        [JsonProperty(PropertyName = "Attribute_Type")]
        public string AttributeTypeName { get; set; }
        [JsonProperty(PropertyName = "Prevent_Multiple_Selection")]
        public bool PreventMultipleSelection { get; set; }
        [JsonProperty(PropertyName = "End_Date")]
        public DateTime? EndDate { get; set; }
        [JsonProperty(PropertyName = "Start_Date")]
        public DateTime? StartDate { get; set; }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (!_unmappedData.Any())
            {
                return;
            }

            MapAttributeType();
        }

        private void MapAttributeType()
        {
            Type = new MpAttributeTypeDto
            {
                Id = _unmappedData.GetUnmappedDataField<int>("Attribute_Type_ID"),
                Name = _unmappedData.GetUnmappedDataField<string>("Attribute_Type_Name")
            };
        }
    }
}

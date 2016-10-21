using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using MinistryPlatform.Translation.Models.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static MinistryPlatform.Translation.Extensions.JsonUnmappedDataExtensions;

namespace MinistryPlatform.Translation.Models.DTO
{
    [MpRestApiTable(Name = "Groups")]
    public class MpGroupDto
    {
        [JsonExtensionData]
#pragma warning disable 649
        private IDictionary<string, JToken> _unmappedData;
#pragma warning restore 649

        [JsonProperty("Group_ID")]
        public int Id { get; set; }

        [JsonProperty("Group_Name")]
        public string Name { get; set; }

        [JsonIgnore]
        public MpAttributeDto AgeRange { get; set; }

        [JsonIgnore]
        public MpAttributeDto Grade { get; set; }

        [JsonIgnore]
        public MpAttributeDto BirthMonth { get; set; }

        [JsonIgnore]
        public MpAttributeDto NurseryMonth { get; set; }

        public MpGroupDto SetKidsClubAttributes(List<MpAttributeDto> attributes)
        {
            if (attributes != null && attributes.Any())
            {
                AgeRange = attributes.Find(a => a.Type.Id == 102);
                Grade = attributes.Find(a => a.Type.Id == 104);
                BirthMonth = attributes.Find(a => a.Type.Id == 103);
                NurseryMonth = attributes.Find(a => a.Type.Id == 105);
            }

            return this;
        }

        public bool HasAgeRange()
        {
            return AgeRange != null; 
        }

        public bool HasGrade()
        {
            return Grade != null;
        }
        public bool HasBirthMonth()
        {
            return BirthMonth != null;
        }
        public bool HasNurseryMonth()
        {
            return NurseryMonth != null;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (!_unmappedData.Any())
            {
                return;
            }

            AgeRange = MapAttribute("Age_Range");
            Grade = MapAttribute("Grade");
            BirthMonth = MapAttribute("Birth_Month");
            NurseryMonth = MapAttribute("Nursery_Month");
        }

        private MpAttributeDto MapAttribute(string attributePrefix)
        {
            if (!_unmappedData.ContainsKey($"{attributePrefix}_Attribute_Name"))
            {
                return null;
            }

            var attr = new MpAttributeDto
            {
                Type = new MpAttributeTypeDto()
            };

            attr.Type.Id = _unmappedData.GetUnmappedDataField<int>($"{attributePrefix}_Attribute_Type_ID");
            attr.Type.Name = _unmappedData.GetUnmappedDataField<string>($"{attributePrefix}_Attribute_Type_Name");

            attr.Name = _unmappedData.GetUnmappedDataField<string>($"{attributePrefix}_Attribute_Name");
            attr.Id = _unmappedData.GetUnmappedDataField<int>($"{attributePrefix}_Attribute_ID");
            attr.SortOrder = _unmappedData.GetUnmappedDataField<int>($"{attributePrefix}_Attribute_Sort_Order");

            return attr;
        }
    }
}

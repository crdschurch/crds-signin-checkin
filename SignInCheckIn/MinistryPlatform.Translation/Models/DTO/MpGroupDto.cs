using System.Collections.Generic;
using System.Runtime.Serialization;
using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static MinistryPlatform.Translation.Extensions.JsonUnmappedDataExtensions;

namespace MinistryPlatform.Translation.Models.DTO
{
    [MpRestApiTable(Name = "Groups")]
    public class MpGroupDto : MpBaseDto
    {
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

        public int SortOrder { get; set; }

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

        protected override void ProcessUnmappedData(IDictionary<string, JToken> unmappedData, StreamingContext context)
        {
            AgeRange = unmappedData.GetAttribute("Age_Range");
            Grade = unmappedData.GetAttribute("Grade");
            BirthMonth = unmappedData.GetAttribute("Birth_Month");
            NurseryMonth = unmappedData.GetAttribute("Nursery_Month");
        }
    }
}

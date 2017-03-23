using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;

namespace MinistryPlatform.Translation.Models.DTO
{
    [MpRestApiTable(Name = "Households")]
    public class MpHouseholdDto
    {
        [JsonProperty(PropertyName = "Household_ID")]
        public int HouseholdId { get; set; }

        [JsonProperty(PropertyName = "Home_Phone")]
        public string HomePhone { get; set; }

        [JsonProperty(PropertyName = "Household_Name")]
        public string HouseholdName { get; set; }

        [JsonProperty(PropertyName = "Congregation_ID")]
        public int CongregationId { get; set; }

        [JsonProperty(PropertyName = "Household_Source_ID")]
        public int HouseholdSourceId { get; set; }
    }
}

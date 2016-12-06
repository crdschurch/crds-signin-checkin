using System;
using MinistryPlatform.Translation.Models.Attributes;
using Newtonsoft.Json;

namespace MinistryPlatform.Translation.Models.DTO
{
    [MpRestApiTable(Name = "Contacts")]
    public class MpContactDto
    {
        [JsonProperty(PropertyName = "Contact_ID")]
        public int ContactId { get; set; }

        [JsonProperty(PropertyName = "Household_ID")]
        public int HouseholdId { get; set; }

        [JsonProperty(PropertyName = "Household_Position_ID")]
        public int HouseholdPositionId { get; set; }

        [JsonProperty(PropertyName = "Home_Phone")]
        public string HomePhone { get; set; }

        [JsonProperty(PropertyName = "Mobile_Phone")]
        public string MobilePhone { get; set; }

        [JsonProperty(PropertyName = "First_Name")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "Nickname")]
        public string Nickname { get; set; }

        [JsonProperty(PropertyName = "Last_Name")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "Display_Name")]
        public string DisplayName { get; set; }

        [JsonProperty(PropertyName = "Company")]
        public bool Company { get; set; }

        [JsonProperty(PropertyName = "Date_of_Birth")]
        public DateTime DateOfBirth { get; set; }
    }
}

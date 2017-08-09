using System;
using Crossroads.Web.Common.MinistryPlatform;
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

        [JsonProperty(PropertyName = "Gender_ID")]
        public int? GenderId { get; set; }

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
        public DateTime? DateOfBirth { get; set; }

        [JsonProperty(PropertyName = "Address_Line_1")]
        public string AddressLine1 { get; set; }

        [JsonProperty(PropertyName = "Address_Line_2")]
        public string AddressLine2 { get; set; }

        [JsonProperty(PropertyName = "City")]
        public string City { get; set; }

        [JsonProperty(PropertyName = "State")]
        public string State { get; set; }

        [JsonProperty(PropertyName = "Postal_Code")]
        public string ZipCode { get; set; }
        
        [JsonProperty(PropertyName = "Congregation_Name")]
        public string CongregationName { get; set; }

        [JsonProperty(PropertyName = "Email_Address")]
        public string EmailAddress { get; set; }
    }
}

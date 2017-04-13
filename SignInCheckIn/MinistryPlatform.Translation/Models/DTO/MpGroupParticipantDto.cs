using System;
using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;

namespace MinistryPlatform.Translation.Models.DTO
{
    [MpRestApiTable(Name = "Group_Participants")]
    public class MpGroupParticipantDto
    {
        [JsonProperty("Group_Participant_ID")]
        public int GroupParticipantId { get; set; }

        [JsonProperty("Group_ID")]
        public int? GroupId { get; set; }

        [JsonProperty("Year_Grade")]
        public int? YearGrade { get; set; }

        [JsonProperty("Contact_ID")]
        public int ContactId { get; set; }

        [JsonProperty("Participant_ID")]
        public int ParticipantId { get; set; }

        [JsonProperty("Group_Role_ID")]
        public int GroupRoleId { get; set; }

        [JsonProperty("Start_Date")]
        public DateTime StartDate { get; set; }

        [JsonProperty("Employee_Role")]
        public bool EmployeeRole { get; set; }

        [JsonProperty("Auto_Promote")]
        public bool AutoPromote { get; set; }

        [JsonProperty(PropertyName = "First_Name")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "Last_Name")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "Nickname")]
        public string Nickname { get; set; }

        [JsonProperty(PropertyName = "Household_ID")]
        public int HouseholdId { get; set; }

        [JsonProperty(PropertyName = "Household_Position_ID")]
        public int HouseholdPositionId { get; set; }

        [JsonProperty(PropertyName = "Date_of_Birth")]
        public DateTime? DateOfBirth { get; set; }

        [JsonProperty(PropertyName = "Group_Type_ID")]
        public int? GroupTypeId { get; set; }

        [JsonProperty(PropertyName = "Gender_ID")]
        public int? GenderId { get; set; }
    }
}

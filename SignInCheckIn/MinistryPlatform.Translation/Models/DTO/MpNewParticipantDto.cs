using System;
using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;

namespace MinistryPlatform.Translation.Models.DTO
{
    [MpRestApiTable(Name = "Participants")]
    public class MpNewParticipantDto
    {
        [JsonProperty(PropertyName = "Event_Participant_ID")]
        public int EventParticipantId { get; set; }

        [JsonProperty(PropertyName = "Participant_ID")]
        public int ParticipantId { get; set; }

        [JsonProperty(PropertyName = "Household_ID")]
        public int HouseholdId { get; set; }

        [JsonProperty(PropertyName = "Household_Position_ID")]
        public int HouseholdPositionId { get; set; }

        [JsonProperty(PropertyName = "First_Name")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "Last_Name")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "Nickname")]
        public string Nickname { get; set; }

        [JsonProperty(PropertyName = "Participation_Status_ID")]
        public int ParticipationStatusId { get; set; }

        [JsonProperty(PropertyName = "Group_ID")]
        public int? GroupId { get; set; }

        [JsonProperty(PropertyName = "Participant_Type_ID")]
        public int ParticipantTypeId { get; set; }

        // TODO: Make sure this is created properly in MP
        [JsonProperty(PropertyName = "Contact_ID")]
        public MpContactDto Contact { get; set; }

        [JsonProperty(PropertyName = "Participant_Contact_ID")]
        public int? ContactId { get; set; }

        [JsonProperty(PropertyName = "Participant_Start_Date")]
        public DateTime ParticipantStartDate { get; set; }

        [JsonIgnore]
        public int? GradeGroupAttributeId { get; set; }
    }
}

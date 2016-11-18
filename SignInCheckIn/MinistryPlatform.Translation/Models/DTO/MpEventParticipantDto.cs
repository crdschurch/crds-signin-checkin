using System;
using MinistryPlatform.Translation.Models.Attributes;
using Newtonsoft.Json;

namespace MinistryPlatform.Translation.Models.DTO
{
    [MpRestApiTable(Name = "Event_Participants")]
    public class MpEventParticipantDto
    {
        [JsonProperty("Event_Participant_ID")]
        public int EventParticipantId { get; set; }

        [JsonProperty("Event_ID")]
        public int EventId { get; set; }

        [JsonProperty("Participant_ID")]
        public int ParticipantId { get; set; }

        [JsonProperty("First_Name")]
        public string FirstName { get; set; }

        [JsonProperty("Last_Name")]
        public string LastName { get; set; }

        [JsonProperty("Participation_Status_ID")]
        public int ParticipantStatusId { get; set; }

        [JsonProperty("Time_In")]
        public DateTime? TimeIn { get; set; } = null;

        [JsonProperty("Time_Confirmed")]
        public DateTime? TimeConfirmed { get; set; } = null;

        [JsonProperty("Time_Out")]
        public DateTime? TimeOut { get; set; } = null;

        [JsonProperty("Notes")]
        public int Notes { get; set; }

        [JsonProperty("Group_Participant_ID")]
        public int? GroupParticipantId { get; set; }

        [JsonProperty("Check-in_Station")]
        public int? CheckinStation { get; set; }

        [JsonProperty("Group_ID")]
        public int? GroupId { get; set; }

        [JsonProperty("Room_ID")]
        public int? RoomId { get; set; }

        [JsonProperty("Call_Parents")]
        public bool CallParents { get; set; }

        [JsonProperty("Group_Role_ID")]
        public int? GroupRoleId { get; set; }

        [JsonProperty("Response_ID")]
        public int? ResponseId { get; set; }

        [JsonProperty("Opportunity_ID")]
        public int? OpportunityId { get; set; }

        [JsonProperty("Registrant_Message_Sent")]
        public bool RegistrantMessageSent { get; set; }
    }
}

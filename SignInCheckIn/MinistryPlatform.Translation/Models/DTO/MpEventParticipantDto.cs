using MinistryPlatform.Translation.Models.Attributes;
using Newtonsoft.Json;

namespace MinistryPlatform.Translation.Models.DTO
{

                //"Event_Participants_ID_Table.Registrant_Message_Sent"

    [MpRestApiTable(Name = "Event_Participants")]
    public class MpEventParticipantDto
    {
        [JsonProperty("Event_Participant_ID")]
        public int EventParticipantId { get; set; }

        [JsonProperty("Event_ID")]
        public int EventId { get; set; }

        [JsonProperty("Participant_ID")]
        public int ParticipantId { get; set; }

        [JsonProperty("Participant_Status_ID")]
        public int ParticipantStatusId { get; set; }

        [JsonProperty("Time_In")]
        public int TimeIn { get; set; }

        [JsonProperty("Time_Confirmed")]
        public int TimeConfirmed { get; set; }

        [JsonProperty("Time_Out")]
        public int TimeOut { get; set; }

        [JsonProperty("Notes")]
        public int Notes { get; set; }

        [JsonProperty("Group_Participant_ID")]
        public int GroupParticipantId { get; set; }

        [JsonProperty("Check-in_Station")]
        public int CheckinStation { get; set; }

        [JsonProperty("Group_ID")]
        public int GroupId { get; set; }

        [JsonProperty("Room_ID")]
        public int RoomId { get; set; }

        [JsonProperty("Call_Parents")]
        public int CallParents { get; set; }

        [JsonProperty("Group_Role_ID")]
        public int GroupRoleId { get; set; }

        [JsonProperty("Response_ID")]
        public int ResponseId { get; set; }

        [JsonProperty("Opportunity_ID")]
        public int OpportunityId { get; set; }

        [JsonProperty("Registrant_Message_Sent")]
        public int RegistrantMessageSent { get; set; }
    }
}

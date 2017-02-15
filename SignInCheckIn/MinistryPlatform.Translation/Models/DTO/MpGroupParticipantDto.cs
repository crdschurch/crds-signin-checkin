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
        public int GroupId { get; set; }

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
    }
}

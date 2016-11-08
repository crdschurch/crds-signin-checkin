using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinistryPlatform.Translation.Models.Attributes;
using Newtonsoft.Json;

namespace MinistryPlatform.Translation.Models.DTO
{
    [MpRestApiTable(Name = "cr_Bumping_Rules")]
    public class MpBumpingRuleDto
    {
        [JsonProperty(PropertyName = "Bumping_Rules_ID")]
        public int BumpingRuleId { get; set; }

        [JsonProperty(PropertyName = "From_Event_Room_ID")]
        public int FromEventRoomId { get; set; }

        [JsonProperty(PropertyName = "Room_Name")]
        public string RoomName { get; set; }

        [JsonProperty(PropertyName = "To_Event_Room_ID")]
        public int ToEventRoomId { get; set; }

        [JsonProperty(PropertyName = "Priority_Order")]
        public int PriorityOrder { get; set; }

        [JsonProperty(PropertyName = "Bumping_Rule_Type_ID")]
        public int BumpingRuleTypeId { get; set; }

        [JsonProperty(PropertyName = "Description")]
        public string BumpingRuleTypeDescription { get; set; }

        [JsonProperty(PropertyName = "From_Room_Name")]
        public string FromRoomName { get; set; }

        [JsonProperty(PropertyName = "From_Room_Number")]
        public int? FromRoomNumber { get; set; }

        [JsonProperty(PropertyName = "To_Room_Name")]
        public string ToRoomName { get; set; }

        [JsonProperty(PropertyName = "To_Room_Number")]
        public int? ToRoomNumber { get; set; }
    }
}

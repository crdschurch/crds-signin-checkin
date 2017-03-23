using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;

namespace MinistryPlatform.Translation.Models.DTO
{
    [MpRestApiTable(Name = "cr_Bumping_Rules")]
    public class MpBumpingRoomsDto
    {
        [JsonProperty(PropertyName = "To_Event_Room_ID")]
        public int EventRoomId { get; set; }

        [JsonProperty(PropertyName = "Room_ID")]
        public int RoomId { get; set; }

        [JsonProperty(PropertyName = "Priority_Order")]
        public int PriorityOrder { get; set; }

        [JsonProperty(PropertyName = "Bumping_Rule_Type_ID")]
        public int BumpingRuleTypeId { get; set; }

        [JsonProperty(PropertyName = "Allow_Checkin")]
        public bool AllowSignIn { get; set; }

        [JsonProperty(PropertyName = "Capacity")]
        public int Capacity { get; set; }

        [JsonProperty(PropertyName = "Room_Name")]
        public string RoomName { get; set; }

        [JsonProperty(PropertyName = "Signed_In")]
        public int SignedIn { get; set; }

        [JsonProperty(PropertyName = "Checked_In")]
        public int CheckedIn { get; set; }
    }
}

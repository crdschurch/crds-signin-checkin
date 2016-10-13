using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinistryPlatform.Translation.Models.Attributes;
using Newtonsoft.Json;

namespace MinistryPlatform.Translation.Models.DTO
{
    [MpRestApiTable(Name = "Event_Rooms")]
    public class MpEventRoomDto
    {
        [JsonProperty(PropertyName = "Event_Room_ID")]
        public int? EventRoomId { get; set; }

        [JsonProperty(PropertyName = "Room_ID")]
        public int RoomId { get; set; }

        [JsonProperty(PropertyName = "Event_ID")]
        public int EventId { get; set; }

        public string RoomName { get; set; }

        public string RoomNumber { get; set; }

        public bool AllowSignIn { get; set; }

        public int Volunteers { get; set; }

        public int Capacity { get; set; }

        public int SignedIn { get; set; }

        public int CheckedIn { get; set; }

    }
}

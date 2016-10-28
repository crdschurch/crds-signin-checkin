using System.Collections.Generic;
using System.Runtime.Serialization;
using MinistryPlatform.Translation.Extensions;
using MinistryPlatform.Translation.Models.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MinistryPlatform.Translation.Models.DTO
{
    [MpRestApiTable(Name = "Event_Groups")]
    public class MpEventGroupDto : MpBaseDto
    {
        [JsonProperty("Event_Group_ID")]
        public int Id { get; set; }
        [JsonIgnore]
        public MpEventDto Event { get; set; } = new MpEventDto();
        [JsonIgnore]
        public MpGroupDto Group { get; set; } = new MpGroupDto();
        [JsonIgnore]
        public MpEventRoomDto RoomReservation { get; set; } = new MpEventRoomDto();

        [JsonProperty("Event_ID")]
        public int EventId
        {
            get { return Event.EventId; }
            set { Event.EventId = value; RoomReservation.EventId = value; }
        }

        [JsonProperty("Group_ID")]
        public int GroupId
        {
            get { return Group.Id; }
            set { Group.Id = value; }
        }

        [JsonProperty("Event_Room_ID")]
        public int? RoomReservationId
        {
            get { return RoomReservation.EventRoomId; }
            set { RoomReservation.EventRoomId = value; }
        }

        [JsonProperty("Room_ID", NullValueHandling = NullValueHandling.Ignore)]
        public int? RoomId
        {
            get { return RoomReservation.RoomId == 0 ? (int?)null : RoomReservation.RoomId; }
            set { RoomReservation.RoomId = value ?? 0; }
        }

        public bool HasRoomReservation()
        {
            return RoomReservation != null && RoomReservation.RoomId > 0;
        }
    }
}

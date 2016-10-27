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

        [JsonProperty("Room_ID")]
        public int RoomId
        {
            get { return RoomReservation.RoomId; }
            set { RoomReservation.RoomId = value; }
        }

        public bool HasRoomReservation()
        {
            return RoomReservation != null && RoomReservation.RoomId > 0;
        }

        protected override void ProcessUnmappedData(IDictionary<string, JToken> unmappedData, StreamingContext context)
        {
            //Event = new MpEventDto
            //{
            //    EventId = unmappedData.GetUnmappedDataField<int>("Event_ID")
            //};

            //Group = new MpGroupDto
            //{
            //    Id = unmappedData.GetUnmappedDataField<int>("Group_ID")
            //};

            //if (unmappedData.ContainsKey("Event_Room_ID"))
            //{
            //    RoomReservation.EventRoomId = unmappedData.GetUnmappedDataField<int>("Event_Room_ID");
            //}
            //if (unmappedData.ContainsKey("Event_ID"))
            //{
            //    RoomReservation.EventRoomId = unmappedData.GetUnmappedDataField<int>("Event_ID");
            //}
            //if (unmappedData.ContainsKey("Room_ID"))
            //{
            //    RoomReservation.EventRoomId = unmappedData.GetUnmappedDataField<int>("Room_ID");
            //}

            //RoomReservation = new MpEventRoomDto
            //{
            //    EventRoomId = unmappedData.GetUnmappedDataField<int>("Event_Room_ID"),
            //    EventId = unmappedData.GetUnmappedDataField<int>("Event_ID"),
            //    RoomId = unmappedData.GetUnmappedDataField<int>("Room_ID")
            //};

        }
    }
}

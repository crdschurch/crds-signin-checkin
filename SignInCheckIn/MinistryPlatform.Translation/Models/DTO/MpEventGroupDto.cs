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
        public MpEventDto Event { get; set; }
        [JsonIgnore]
        public MpGroupDto Group { get; set; }
        [JsonIgnore]
        public MpEventRoomDto RoomReservation { get; set; }

        public bool HasRoomReservation()
        {
            return RoomReservation != null && RoomReservation.RoomId > 0;
        }

        protected override void ProcessUnmappedData(IDictionary<string, JToken> unmappedData, StreamingContext context)
        {
            Event = new MpEventDto
            {
                EventId = unmappedData.GetUnmappedDataField<int>("Event_ID")
            };

            Group = new MpGroupDto
            {
                Id = unmappedData.GetUnmappedDataField<int>("Group_ID")
            };

            RoomReservation = new MpEventRoomDto
            {
                EventRoomId = unmappedData.GetUnmappedDataField<int>("Event_Room_ID"),
                EventId = unmappedData.GetUnmappedDataField<int>("Event_ID"),
                RoomId = unmappedData.GetUnmappedDataField<int>("Room_ID")
            };
        }
    }
}

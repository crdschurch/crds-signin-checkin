using MinistryPlatform.Translation.Models.Attributes;
using Newtonsoft.Json;

namespace MinistryPlatform.Translation.Models.DTO
{
    [MpRestApiTable(Name = "Event_Rooms")]
    public class MpEventRoomDto
    {
        [JsonProperty("Event_Room_ID", NullValueHandling = NullValueHandling.Ignore)]
        public int? EventRoomId { get; set; }

        [JsonProperty("Room_ID")]
        public int RoomId { get; set; }

        [JsonProperty("Event_ID")]
        public int EventId { get; set; }

        [JsonProperty("Room_Name", NullValueHandling = NullValueHandling.Ignore)]
        public string RoomName { get; set; }

        [JsonProperty("Room_Number", NullValueHandling = NullValueHandling.Ignore)]
        public string RoomNumber { get; set; }

        [JsonProperty("Allow_Checkin")]
        public bool AllowSignIn { get; set; }

        [JsonProperty("Volunteers")]
        public int Volunteers { get; set; }

        [JsonProperty("Capacity")]
        public int Capacity { get; set; }

        public int SignedIn { get; set; }

        public int CheckedIn { get; set; }

        [JsonProperty("Hidden")]
        public bool Hidden { get; set; } = false;

    }
}

using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;

namespace MinistryPlatform.Translation.Models.DTO
{
    [MpRestApiTable(Name = "Event_Rooms")]
    public class MpEventRoomDto
    {
        [MpRestApiPrimaryKey("Event_Room_ID")]
        [JsonProperty(PropertyName = "Event_Room_ID", NullValueHandling = NullValueHandling.Ignore)]
        public int? EventRoomId { get; set; }

        [JsonProperty(PropertyName = "Room_ID")]
        public int RoomId { get; set; }

        [JsonProperty(PropertyName = "Event_ID")]
        public int EventId { get; set; }

        [JsonProperty(PropertyName = "Room_Name")]
        public string RoomName { get; set; }

        [JsonProperty(PropertyName = "KC_Sort_Order")]
        public int? KcSortOrder { get; set; }

        [JsonProperty(PropertyName = "Room_Number")]
        public string RoomNumber { get; set; }

        [JsonProperty(PropertyName = "Allow_Checkin")]
        public bool AllowSignIn { get; set; }

        [JsonProperty(PropertyName = "Volunteers")]
        public int? Volunteers { get; set; }

        [JsonProperty(PropertyName = "Capacity")]
        public int? Capacity { get; set; }

        [JsonProperty(PropertyName = "Signed_In")]
        public int? SignedIn { get; set; }

        [JsonProperty(PropertyName = "Checked_In")]
        public int? CheckedIn { get; set; }

        [JsonProperty(PropertyName = "Label")]
        public string Label { get; set; }

        [JsonProperty(PropertyName = "Hidden")]
        public bool Hidden { get; set; } = false;
        
    }
}

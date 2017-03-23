using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;

namespace MinistryPlatform.Translation.Models.DTO
{
    [MpRestApiTable(Name = "Rooms")]
    public class MpRoomDto
    {
        [JsonProperty(PropertyName = "Room_ID")]
        public int RoomId { get; set; }

        [JsonProperty(PropertyName = "Room_Name")]
        public string RoomName { get; set; }

        [JsonProperty(PropertyName = "Room_Number")]
        public string RoomNumber { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinistryPlatform.Translation.Models.Attributes;
using Newtonsoft.Json;

namespace MinistryPlatform.Translation.Models.DTO
{
    [MpRestApiTable(Name = "Events")]
    public class MpEventDto
    {
        [JsonProperty(PropertyName = "Event_ID")]
        public int EventId { get; set; }

        [JsonProperty(PropertyName = "Event_Title")]
        public string EventTitle { get; set; }

        [JsonProperty(PropertyName = "Event_Start_Date")]
        public DateTime EventStartDate { get; set; }

        [JsonProperty(PropertyName = "Event_Type")]
        public string EventType { get; set; }
    }
}

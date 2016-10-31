using System;
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

        [JsonProperty(PropertyName = "Event_End_Date")]
        public DateTime EventEndDate { get; set; }

        [JsonProperty(PropertyName = "Event_Type")]
        public string EventType { get; set; }

        [JsonProperty(PropertyName = "Early_Check-in_Period")]
        public int? EarlyCheckinPeriod { get; set; }

        [JsonProperty(PropertyName = "Late_Check-in_Period")]
        public int? LateCheckinPeriod { get; set; }

        [JsonProperty(PropertyName = "Congregation_Name")]
        public string CongregationName { get; set; }

        [JsonProperty(PropertyName = "Congregation_ID")]
        public string CongregationId { get; set; }

        [JsonProperty(PropertyName = "Location_ID")]
        public int LocationId { get; set; }
    }
}

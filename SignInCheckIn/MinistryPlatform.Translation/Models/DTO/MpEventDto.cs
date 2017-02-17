using System;
using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;

namespace MinistryPlatform.Translation.Models.DTO
{
    [MpRestApiTable(Name = "Events")]
    public class MpEventDto
    {
        [JsonProperty(PropertyName = "Event_ID")]
        public int EventId { get; set; }

        [JsonProperty(PropertyName = "Parent_Event_ID")]
        public int? ParentEventId { get; set; }

        [JsonProperty(PropertyName = "Event_Title")]
        public string EventTitle { get; set; }

        [JsonProperty(PropertyName = "Event_Start_Date")]
        public DateTime EventStartDate { get; set; }

        [JsonProperty(PropertyName = "Event_End_Date")]
        public DateTime EventEndDate { get; set; }

        [JsonProperty(PropertyName = "Event_Type")]
        public string EventType { get; set; }

        [JsonProperty(PropertyName = "Event_Type_ID")]
        public int EventTypeId { get; set; }

        [JsonProperty(PropertyName = "Early_Check-in_Period")]
        public int? EarlyCheckinPeriod { get; set; }

        [JsonProperty(PropertyName = "Late_Check-in_Period")]
        public int? LateCheckinPeriod { get; set; }

        [JsonProperty(PropertyName = "Congregation_Name")]
        public string CongregationName { get; set; }

        [JsonProperty(PropertyName = "Congregation_ID")]
        public int CongregationId { get; set; }

        [JsonProperty(PropertyName = "Location_ID")]
        public int LocationId { get; set; }

        [JsonProperty(PropertyName = "Program_ID")]
        public int ProgramId { get; set; }

        [JsonProperty(PropertyName = "Primary_Contact")]
        public int PrimaryContact { get; set; }

        [JsonProperty(PropertyName = "Minutes_for_Setup")]
        public int MinutesForSetup { get; set; }

        [JsonProperty(PropertyName = "Minutes_for_Cleanup")]
        public int MinutesForCleanup { get; set; }

        [JsonProperty(PropertyName = "Cancelled")]
        public bool Cancelled { get; set; }

        [JsonProperty(PropertyName = "Allow_Check-in")]
        public bool AllowCheckIn { get; set; }
    }
}

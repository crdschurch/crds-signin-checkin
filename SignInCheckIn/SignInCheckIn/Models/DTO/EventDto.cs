using System;

namespace SignInCheckIn.Models.DTO
{
    public class EventDto
    {
        public int EventId { get; set; }
        public string EventTitle { get; set; }
        public DateTime EventStartDate { get; set; }
        public DateTime EventEndDate { get; set; }
        public string EventType { get; set; }
        public string EventSite { get; set; }
        public int EventSiteId { get; set; }
        public int? EarlyCheckinPeriod { get; set; }
        public int? LateCheckinPeriod { get; set; }
    }
}
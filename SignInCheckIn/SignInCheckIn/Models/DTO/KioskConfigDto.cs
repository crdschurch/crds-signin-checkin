using System;

namespace SignInCheckIn.Models.DTO
{
    public class KioskConfigDto
    {
        public int KioskConfigId { get; set; }

        public Guid KioskIdentifier { get; set; }

        public string KioskName { get; set; }

        public string KioskDescription { get; set; }

        public int KioskTypeId { get; set; }

        public int LocationId { get; set; }

        public int CongregationId { get; set; }

        public string CongregationName { get; set; }

        public int RoomId { get; set; }

        public string RoomName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
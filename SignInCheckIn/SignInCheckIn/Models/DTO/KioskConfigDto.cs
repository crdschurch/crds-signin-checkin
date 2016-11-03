using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

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

        public int RoomId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
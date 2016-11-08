using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignInCheckIn.Models.DTO
{
    public class BumpingRuleDto
    {
        public int BumpingRuleId { get; set; }

        public int FromEventRoomId { get; set; }

        public string RoomName { get; set; }

        public int ToEventRoomId { get; set; }

        public int PriorityOrder { get; set; }

        public int BumpingRuleTypeId { get; set; }

        public string BumpingRuleTypeDescription { get; set; }

        public string FromRoomName { get; set; }

        public int FromRoomNumber { get; set; }

        public string ToRoomName { get; set; }

        public int ToRoomNumber { get; set; }
    }
}
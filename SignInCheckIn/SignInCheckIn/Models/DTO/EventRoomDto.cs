using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignInCheckIn.Models.DTO
{
    public class EventRoomDto
    {
        public int? EventRoomId { get; set; }

        public int RoomId { get; set; }

        public int EventId { get; set; }

        public string RoomName { get; set; }

        public string RoomNumber { get; set; }

        public bool AllowSignIn { get; set; }

        public int Volunteers { get; set; }

        public int Capacity { get; set; }

        public int SignedIn { get; set; }

        public int CheckedIn { get; set; }

        public string Label { get; set; }
    }
}